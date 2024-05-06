using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Model;
using ChatApiApplication.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApiApplication.Controllers
{
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
        public async Task<IActionResult> RegisterUser(ChatUsersDTO usersDTO)
        {
            bool isEmailUnique = await _us.IsEmailUniqueAsync(usersDTO.Email);
            if (!isEmailUnique)
            {
                return BadRequest("Registration failed because the email is already registered");
            }
            else if (isEmailUnique && !string.IsNullOrWhiteSpace(usersDTO.Email))
            {
               await _us.AddUserAsync(usersDTO);
                var userDtoResponse = new ChatUsersDTO
                {
                    UserId = usersDTO.UserId,
                    UserName = usersDTO.UserName,
                    Email = usersDTO.Email,
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
