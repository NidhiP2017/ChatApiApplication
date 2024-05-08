using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Model;
using ChatApiApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApiApplication.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/")]
    public class ChatUsersController : Controller
    {
        public readonly IChatUserService _us;

        public ChatUsersController(IChatUserService userservice)
        {
            _us = userservice;
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

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var GetAllUsersAsync = await _us.GetAllUsersAsync();
            return Ok(GetAllUsersAsync);
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
