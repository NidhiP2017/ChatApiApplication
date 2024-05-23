using ChatApiApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using ChatApiApplication.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace ChatApiApplication.CustomMiddleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    public AuthMiddleware(RequestDelegate _next, IConfiguration _configuration)
    {
        this._next = _next;
        this._configuration = _configuration;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var endpoint = httpContext.GetEndpoint();
        var isAllowAnonymous = endpoint?.Metadata.Any(x => x.GetType() == typeof(AllowAnonymousAttribute)) ?? false;

        if (isAllowAnonymous)
        {
            await _next(httpContext);
            return;
        }
        if (!httpContext.Request.Headers.ContainsKey("Authorization"))
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            throw new NotFoundException("401 - Authorization failed: Token is missing.");
        }

        var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var isTokenValid = CheckTokenIsValid(token);
        if (isTokenValid == false)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Gone;
            await httpContext.Response.WriteAsync("410 - Token Expired. Please login again.");
            return;
        }

        if (!string.IsNullOrEmpty(token) && isTokenValid)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["AuthSettings:Key"]);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["AuthSettings:Issuer"],
                    ValidAudience = _configuration["AuthSettings:Audience"],
                    ClockSkew = TimeSpan.FromDays(1)
                };

                ISecurityTokenValidator tokenValidator = new JwtSecurityTokenHandler();
                var claim = tokenValidator.ValidateToken(token, validationParameters, out _);

                ITenantContext? tenantContext = httpContext.RequestServices.GetService(typeof(ITenantContext)) as ITenantContext;
                tenantContext?.SetValues(claim.Claims);
            }
            catch (System.Exception ex)
            {
                httpContext.Response.StatusCode = 400; // Bad Request
                await httpContext.Response.WriteAsync("400 - Bad Request");
                return;
            }
        }
        else
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await httpContext.Response.WriteAsync("401 - Authorization failed");
            return;
        }

        await _next(httpContext);


    }
    public static long GetTokenExpirationTime(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
        var ticks = long.Parse(tokenExp);
        return ticks;
    }
    public static bool CheckTokenIsValid(string token)
    {
        var tokenTicks = GetTokenExpirationTime(token);
        var tokenDate = DateTimeOffset.FromUnixTimeSeconds(tokenTicks).UtcDateTime;
        var now = DateTime.UtcNow;
        var valid = tokenDate >= now;
        return valid;
    }
}
