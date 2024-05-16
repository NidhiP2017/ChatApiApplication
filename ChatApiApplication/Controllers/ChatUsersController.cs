using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ChatApiApplication.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ChatUsersController : Controller
    {
        public readonly IChatUserService _us;
        public string _jwtToken;
        private readonly ChatAPIDbContext _chatAPIDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatUsersController(IHttpContextAccessor httpContextAccessor, IChatUserService userservice, ChatAPIDbContext chatAPIDbContext)
        {
            _us = userservice;
            /*_httpContextAccessor = httpContextAccessor;
            _jwtToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            _jwtToken = (_jwtToken != null) ? (_jwtToken.Substring("Bearer ".Length).Trim()) : "";*/
            _chatAPIDbContext = chatAPIDbContext;
            //_jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKV1RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiI5ZmZiMmI0NC1lZGMyLTRjMzAtYmM1Mi04MTkzNjQ1Yjc0NzIiLCJpYXQiOiIxMS0wNS0yMDI0IDEwOjU4OjI2IiwiZXhwIjoxNzE1NDI1NzA2LCJpc3MiOiJKV1RBdXRoZW50aWNhdGlvblNlcnZlciIsImF1ZCI6IkpXVFNlcnZpY2VQb3N0bWFuQ2xpZW50In0.Ib5urJOc7eVXvMqQBPkml-cKJWgrIFuIB21cdcO7cjc";
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser(ChatUsersDTO chatUsersDTO)
        {
            bool isEmailUnique = await _us.IsEmailUniqueAsync(chatUsersDTO.Email);
            if (!isEmailUnique)
            {
                return BadRequest("Registration failed because the email is already registered");
            }
            else if (isEmailUnique && !string.IsNullOrWhiteSpace(chatUsersDTO.Email))
            {
                await _us.AddUserAsync(chatUsersDTO);
                var userDtoResponse = new ChatUsersDTO
                {
                    UserId = chatUsersDTO.UserId,
                    UserName = chatUsersDTO.UserName,
                    Email = chatUsersDTO.Email,
                };

                return Ok(userDtoResponse);
            }
            else
            {
                return BadRequest("Registration failed due to validation errors");
            }
            
        }
        [Authorize]
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            if (_jwtToken != null) {
                var uid = new Guid("1ac9689f-155c-4e33-c548-08dc73ea4971"); // temporary static sender ID
                var userId = from u in _chatAPIDbContext.ChatUsers
                                 //where u.AccessToken == _jwtToken
                          where u.UserId == uid
                          select u.UserId;
                var GetAllUsersAsync = await _us.GetAllUsersAsync(userId);
                return Ok(GetAllUsersAsync);
            } 
            else 
            { 
                return BadRequest("Could not Authorize");  
            }
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginUser(ChatUserLoginDTO userDTO)
        {
            var user = await _us.AuthenticateUser(userDTO);
            return Ok(user);
        }

        [HttpGet]
        [Route("conversation/search")]
        public async Task<IActionResult> SearchConversations(string query)
        {
            var msgs = await _us.SearchMsgs(query);
            return Ok(msgs);
        }


    }
}
