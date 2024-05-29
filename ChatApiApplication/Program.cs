using AutoMapper;
using ChatApiApplication;
using ChatApiApplication.Automapper;
using ChatApiApplication.Data;
using ChatApiApplication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using ChatApiApplication.CustomMiddleware;
using Microsoft.AspNetCore.Authentication.Google;
using ChatApiApplication.Hubs;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

var mapperConfiguration = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperProfileConfiguration());
});
IMapper mapper = mapperConfiguration.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddSwaggerGen(options =>
{
    /*options.AddSecurityDefinition("JWT Token(Bearer)", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });*/

      options.AddSecurityDefinition("Google", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
                TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect scope" },
                    { "profile", "Access your basic profile info" },
                    { "email", "Access your email address" }
                }
            }
        }
    });
   options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddDbContext<ChatAPIDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChatAPIConnectionString")));

builder.Services.AddScoped<IChatUserService, ChatUserService>();
builder.Services.AddScoped<IMessagesService, MessagesService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This is my 128 bits very long secret key...............................")),
        ValidateAudience = false,
        ValidateIssuer = false
    };
}).AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = "1055043777002-t3albpi7if9q1537k5abk78q2reqp6ic.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.CallbackPath = "/signin-google";
});
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

// Add HttpContextAccessor
builder.Services.AddSingleton<IUrlHelper>(x =>
{
    var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
    return new UrlHelper(actionContext);
});

/*builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwagger",
        builder => builder.
         WithOrigins("http://localhost:7187")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://localhost:7187") // URL of the client project
         .AllowAnyHeader()
         .AllowAnyMethod());
});*/
builder.Services.AddSingleton<IConnection<string>, connection<string>>();

builder.Services.AddMvc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthentication();
app.UseAuthorization();


app.UseCors("AllowSwagger");
app.MapHub<ChatHub>("/chathub");

app.MapPost("Broadcast", async (string message, ChatHub context) =>
{
    await context.Clients.All.SendAsync("ReceiveMessage", message);
    return Results.NoContent();
});

app.UseMiddleware<CustomMiddleware>();

app.MapControllers();

app.Run();
