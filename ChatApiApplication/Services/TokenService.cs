using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApiApplication.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration _configuration)
        {
            this._configuration = _configuration;
            //this._context = _context;
        }

        public string TokenGenerate(IdentityUser user)
        {
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(_configuration["AuthSettings:Key"]);

            ////Expires token
            //var expires = DateTime.UtcNow.AddDays(1);
            //var claims = new List<Claim> { };

            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(claims),
            //    Expires = expires,
            //    Issuer = _configuration["AuthSettings:Issuer"],
            //    Audience = _configuration["AuthSettings:Audience"],
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            //};

            //var token = tokenHandler.CreateToken(tokenDescriptor);
            //var jwtToken = tokenHandler.CreateEncodedJwt(tokenDescriptor);
            //return jwtToken;
            List<Claim> claims = new List<Claim>
            {
                //new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                 //new Claim("UserId", user.UserID.ToString())
                //new Claim(ClaimTypes.Role,"User")
            };

            //var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("This is my 128 bits very long secret key.......").Value))
            var key = Encoding.ASCII.GetBytes("This is my 128 bits very long secret key.......");
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }       
    }

    public interface ITokenService
    {
        string TokenGenerate(IdentityUser user);
    }

}
