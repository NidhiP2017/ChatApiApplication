using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Model;
using ChatApiApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApiApplication.Controllers
{
    [ApiController]
    [Route("api/")]
    public class UsersController : Controller
    {
        public IUserService _userService;

        public UsersController(IUserService userservice)
        {
            _userService = userservice;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser(UsersDTO usersDTO)
        {
            bool isEmailUnique = await _userService.IsEmailUniqueAsync(usersDTO.Email);
            if (isEmailUnique && !string.IsNullOrWhiteSpace(usersDTO.Email))
            {
                await _userService.AddUserAsync(usersDTO);
                return Ok("User registered successfully");
            }
            else
            {
                return BadRequest("User failed to register");
            }
            
        }

        /*[HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginUser(UsersDTO usersDTO)
        {
            await _userService.LoginUserAsync(usersDTO);
            await _userService.SaveChangesAsync();
            return Ok(UsersDTO);
        }

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetAllUsers(UsersDTO usersDTO)
        {
            return Ok(await _userService.ToListAsync());

        }*/
    }
}
