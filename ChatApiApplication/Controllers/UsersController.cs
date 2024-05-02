using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApiApplication.Controllers
{
    [ApiController]
    [Route("api/")]
    public class UsersController : Controller
    {
        private readonly ChatAPIDbContext dbContext;

        public UsersController(ChatAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser(UsersDTO usersDTO)
        {
            var users = new Users();
            await dbContext.Users.AddAsync(users);
            await dbContext.SaveChangesAsync();
            return Ok(users);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginUser(UsersDTO usersDTO)
        {
            var users = new Users();
            await dbContext.Users.AddAsync(users);
            await dbContext.SaveChangesAsync();
            return Ok(users);
        }

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetAllUsers(UsersDTO usersDTO)
        {
            return Ok(await dbContext.Users.ToListAsync());

        }
    }
}
