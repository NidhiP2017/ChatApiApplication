﻿using AutoMapper;
using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Interfaces;
using ChatApiApplication.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Web.Helpers;
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
        private readonly ITokenService tokenService;
        private readonly IMapper _imapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ChatUserService(IHttpContextAccessor httpContextAccessor ,IMapper imapper, ChatAPIDbContext context, IConfiguration config,
            ITokenService tokenService, IWebHostEnvironment webHostEnvironment)
        {
            _appContext = context;
            _imapper = imapper;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            this.tokenService = tokenService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return await _appContext.ChatUsers.AllAsync(u => u.Email != email);
        }

        public async Task<IActionResult> AddUserAsync(RegisterDto usersDTO)
        {
            var encryptPwdBytes = Encoding.UTF8.GetBytes(usersDTO.Password);
            var encryptedPwd = Convert.ToBase64String(encryptPwdBytes);
            var newUser = new ChatUsers()
            {
                UserName = usersDTO.UserName,
                Email = usersDTO.Email,
                Password = encryptedPwd,
                AccessToken = " ",
                userStatus = usersDTO.userStatus
            };
            await _appContext.ChatUsers.AddAsync(newUser);
            await _appContext.SaveChangesAsync();
            return new OkResult();
        }

        public async Task<IActionResult> GetAllUsersAsync(IQueryable<Guid> userId)
        {
            var users = await _appContext.ChatUsers
                       .Where(u => u.UserId != userId.FirstOrDefault()).ToListAsync();
            var usersList = users.Select(a => new ChatUserListDto
            {
                UserId = a.UserId,
                UserName = a.UserName,
                Email = a.Email,
            });

            return new OkObjectResult(usersList);
        }

        public async Task<IActionResult> AuthenticateUser(LoginDto loginUsersDTO)
        {
            var query = from u in _appContext.ChatUsers
                        where u.Email == loginUsersDTO.Email
                        select u.UserId;
            Guid userId = query.First();

            var user = await _appContext.ChatUsers.FindAsync(userId);
            if (user != null)
            {
                var pwdBytes = Convert.FromBase64String(user.Password);
                var pwdString = Encoding.UTF8.GetString(pwdBytes);

                if (user.Email == loginUsersDTO.Email && loginUsersDTO.Password == pwdString)
                {
                    var userList = new ChatUsersDTO()
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        Email = user.Email
                    };
                    var tokenServices = tokenService.TokenGenerate(user);
                    return new OkObjectResult(
                       new { User = userList,
                        tokenServices });
                }
                else
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
                expires: DateTime.UtcNow.AddDays(2),
                signingCredentials: signIn);

            string AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return AccessToken;
        }

        public async Task<List<MessagesDTO>> SearchMsgs(string msg)
        {
            var Id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) ? .Value;
            Guid currentUserId = await _appContext.ChatUsers.Where(m => m.Id == Id).Select(u => u.UserId).FirstOrDefaultAsync();
            if (currentUserId != null)
            {
                var conversations = await _appContext.Messages
                    .Where(c => (c.SenderId == currentUserId || c.ReceiverId == currentUserId) && c.Content.Contains(msg))
                    .AsNoTracking() //to disconnect from database
                    .ToListAsync();
                if (conversations.Any())
                {
                    List<MessagesDTO> matchedConversations = new List<MessagesDTO>();
                    matchedConversations = _imapper.Map<List<MessagesDTO>>(conversations);
                    return matchedConversations;
                }
                else
                {
                    return new List<MessagesDTO>();
                }
            } 
            else
            {
                return null; 
            }
        }

        public async Task<IActionResult> UpdateStatus(Guid userId, string status)
        {
            if (userId != null)
            {
                var user = await _appContext.ChatUsers.FindAsync(userId);
                user.userStatus = status;
                await _appContext.SaveChangesAsync();
                return new OkObjectResult("Status updated to: "+status);            
            }
            else
            {
                return new OkObjectResult("Could not find user");
            }
        }

        public async Task<IActionResult> UploadPhoto(List<IFormFile> files)
        {
            var Id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid currentUserId = await _appContext.ChatUsers.Where(m => m.Id == Id).Select(u => u.UserId).FirstOrDefaultAsync();

            if (files.Count == 0)
                return new OkObjectResult("No file was uploaded");

            string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadedFiles");
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            foreach (var file in files)
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return new OkObjectResult("Invalid file type.");
                }
                string filePath = Path.Combine(directoryPath, file.FileName+"_"+currentUserId);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var user = await _appContext.ChatUsers.FindAsync(currentUserId);
                user.profilePhoto = Path.Combine(directoryPath, file.FileName);
                await _appContext.SaveChangesAsync();
            }
            return new OkObjectResult("Upload Successful");
        }
    }
}
