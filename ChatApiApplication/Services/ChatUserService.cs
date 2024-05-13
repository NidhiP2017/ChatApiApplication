using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ChatApiApplication.Services
{
    public class ChatUserService : IChatUserService
    {
        private readonly ChatAPIDbContext _appContext;
        private readonly IConfiguration _config;

        public ChatUserService(ChatAPIDbContext context, IConfiguration config)
        {
            _appContext = context;
            _config = config;
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        { 
            return await _appContext.ChatUsers.AllAsync(u => u.Email != email);
        }

        public async Task<IActionResult> AddUserAsync(ChatUsersDTO usersDTO)
        {
            var encryptPwdBytes = Encoding.UTF8.GetBytes(usersDTO.Password);
            var encryptedPwd = Convert.ToBase64String(encryptPwdBytes);
            var newUser = new ChatUsers()
            {
                UserName = usersDTO.UserName,
                Email = usersDTO.Email,
                Password = encryptedPwd,
                AccessToken = GetToken()
            };
            await _appContext.ChatUsers.AddAsync(newUser);
            await _appContext.SaveChangesAsync();
            return new OkResult();
        }

        public async Task<IActionResult> GetAllUsersAsync(IQueryable<Guid> userId)
        {
            var users = await _appContext.ChatUsers
                       .Where(u => u.UserId != userId.FirstOrDefault()).ToListAsync();
            var usersList = users.Select(a => new ChatUsersDTO
            {
                UserId = a.UserId,
                UserName = a.UserName,
                Email = a.Email
            });

            return new OkObjectResult(usersList);
        }
       
        public async Task<IActionResult> AuthenticateUser(ChatUserLoginDTO loginUsersDTO)
        {
            var query = from u in _appContext.ChatUsers
                        where u.Email == loginUsersDTO.Email
                        select u.UserId;
            Guid? userId = query.FirstOrDefault();

            var user = await _appContext.ChatUsers.FindAsync(userId);
            if (user != null)
            {
                var pwdBytes = Convert.FromBase64String(user.Password);
                var pwdString = Encoding.UTF8.GetString(pwdBytes);

                if(user.Email == loginUsersDTO.Email && loginUsersDTO.Password == pwdString)
                {
                    var userList = new ChatUsersDTO()
                    {
                        UserId =user.UserId,
                        UserName = user.UserName,
                        Email=user.Email,
                        AccessToken = user.AccessToken,
                    };
                    return new OkObjectResult(userList);
                }  else
                {
                    return new OkObjectResult("Invalid Credentials");
                }
            }
            return new OkObjectResult("Could not Authenticate User");
        }

        public string GetToken()
        {
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
                        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);


            string AccessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return AccessToken;
        }
    }
}
