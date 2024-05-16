﻿using AutoMapper;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ChatApiApplication.Services
{
    public class ChatUserService : IChatUserService
    {
        private readonly ChatAPIDbContext _appContext;
        private readonly IConfiguration _config;

        public ChatUserService(ChatAPIDbContext context, IConfiguration config )
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
                        Email = user.Email,
                        AccessToken = user.AccessToken,
                    };
                    return new OkObjectResult(userList);
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
            /*var claims = new[] {
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
                expires: DateTime.UtcNow.AddDays(10),
                signingCredentials: signIn);


            string AccessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return "Bearer "+AccessToken;*/

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<List<MessagesDTO>> SearchMsgs(string msg)
        {
            Guid userId = new Guid("1ac9689f-155c-4e33-c548-08dc73ea4971"); //static for now.
            if (userId != null)
            {
                var conversations = await _appContext.Messages
                    .Where(c => (c.SenderId == userId || c.ReceiverId == userId) && c.Content.Contains(msg))
                    .AsNoTracking() //to disconnect from database
                    .ToListAsync();
                if (conversations.Any())
                {
                    List<MessagesDTO> matchedConversations = new List<MessagesDTO>();
                    /*matchedConversations = _mapper.Map<List<MessagesDTO>>(conversations);
                        return matchedConversations;*/
                    foreach (var conversation in conversations)
                    {
                        var messageDTOs = conversations.Select(m => new MessagesDTO
                        {
                            MessageId = conversation.MessageId,
                            Content = conversation.Content,
                            SenderId = conversation.SenderId,
                            ReceiverId = conversation.ReceiverId,
                            Timestamp = conversation.Timestamp
                        }).ToList();
                        return (messageDTOs);
                        /*var newmessageDTOs = new MessagesDTO
                        {
                            MessageId = messageDTOs
                        };
                        matchedConversations.Add(newmessageDTOs);*/
                    }
                    return new List<MessagesDTO>();
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
    }
}
