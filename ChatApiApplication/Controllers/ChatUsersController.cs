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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _jwtToken;
        private readonly ChatAPIDbContext _chatAPIDbContext;

        public ChatUsersController(IHttpContextAccessor httpContextAccessor, IChatUserService userservice, ChatAPIDbContext chatAPIDbContext)
        {
            _us = userservice;
            _httpContextAccessor = httpContextAccessor;
            _jwtToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            _chatAPIDbContext = chatAPIDbContext;
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
        //[Authorize]
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            _jwtToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var userId = from u in _chatAPIDbContext.ChatUsers
                         where u.AccessToken == _jwtToken
                         select u.UserId;
            if (_jwtToken != null)
            {
                var GetAllUsersAsync = await _us.GetAllUsersAsync(userId.ToString());
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
        
    }
}
