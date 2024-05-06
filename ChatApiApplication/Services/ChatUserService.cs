using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApiApplication.Services
{
    public class ChatUserService : IChatUserService
    {
        private readonly ChatAPIDbContext _appContext;

        public ChatUserService(ChatAPIDbContext context)
        {
            _appContext = context;
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        { 
            return await _appContext.ChatUsers.AllAsync(u => u.Email != email);
        }

        public async Task<IActionResult> AddUserAsync(ChatUsersDTO usersDTO)
        {
            var newUser = new ChatUsers()
            {
                UserName = usersDTO.UserName,
                Email = usersDTO.Email,
                Password = usersDTO.Password
            };
            await _appContext.ChatUsers.AddAsync(newUser);
            await _appContext.SaveChangesAsync();
            return new OkResult();
        }

        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _appContext.ChatUsers.ToListAsync();
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
            var user = await _appContext.ChatUsers.FindAsync(loginUsersDTO.Email);
            if (user != null)
            {
                if(user.Email == loginUsersDTO.Email && user.Password == loginUsersDTO.Password)
                {
                    var userList = new ChatUsersDTO()
                    {
                        UserId =user.UserId,
                        UserName = user.UserName,
                        Email=user.Email,
                    };
                    return new OkObjectResult(userList);
                }  else
                {
                    return new OkObjectResult("Invalid Credentials");
                }
            }
            return new OkObjectResult("Could not Authenticate User");
        }
    }
}
