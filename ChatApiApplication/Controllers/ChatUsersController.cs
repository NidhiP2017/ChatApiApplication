using AutoMapper;
using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Web.Helpers;
using System.Net.Http.Headers;
using System.Security.Claims;

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

        [Authorize]
        [HttpPut]
        [Route("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(string status)
        {
            var Id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid currentUserId = await _chatAPIDbContext.ChatUsers.Where(m => m.Id == Id).Select(u => u.UserId).FirstOrDefaultAsync();

            var u = await _us.UpdateStatus(currentUserId, status);
            return Ok(u);
        }

        [Authorize]
        [HttpPost]
        [Route("UploadPhoto")]
        public async Task<IActionResult> Upload([FromForm] List<IFormFile> files)
        {
            var profile = await _us.UploadPhoto(files);
            return Ok(profile);
        }

    }
}
