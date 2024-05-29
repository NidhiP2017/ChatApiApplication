using AutoMapper;
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
        private readonly ChatAPIDbContext _chatAPIDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        

        public ChatUsersController(IHttpContextAccessor httpContextAccessor, IChatUserService userservice, ChatAPIDbContext chatAPIDbContext)
        {
            _us = userservice;
            _chatAPIDbContext = chatAPIDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser(RegisterDto chatUsersDTO)
        {
            bool isEmailUnique = await _us.IsEmailUniqueAsync(chatUsersDTO.Email);
            if (!isEmailUnique)
            {
                return BadRequest("Registration failed because the email is already registered");
            }
            else if (isEmailUnique && !string.IsNullOrWhiteSpace(chatUsersDTO.Email))
            {
                await _us.AddUserAsync(chatUsersDTO);
                var userDtoResponse = new RegisterUserResponse
                {                    
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
                var userId = from u in _chatAPIDbContext.ChatUsers                                 
                          select u.UserId;
                var GetAllUsersAsync = await _us.GetAllUsersAsync(userId);
                return Ok(GetAllUsersAsync);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginUser(LoginDto userDTO)
        {
            var user = await _us.AuthenticateUser(userDTO);
            return Ok(user);
        }
        [Authorize]
        [HttpGet]
        [Route("conversation/search")]
        public async Task<IActionResult> SearchConversations(string query)
        {
            var msgs = await _us.SearchMsgs(query);
            return Ok(msgs);
        }


    }
}
