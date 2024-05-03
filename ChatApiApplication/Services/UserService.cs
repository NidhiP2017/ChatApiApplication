using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApiApplication.Services
{
    public class UserService : IUserService
    {
        private readonly ChatAPIDbContext _context;

        public UserService(ChatAPIDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                return await _context.Users.AllAsync(u => u.Email != email);
            } 
            else
            {
                throw new Exception("Email is not unique");
            }
        }

        public async Task<IActionResult> AddUserAsync(UsersDTO usersDTO)
        {
            
            var newUser = new Users()
            {
                UserName = usersDTO.UserName,
                Email = usersDTO.Email, 
                Password = usersDTO.Password,
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return new OkResult();

        }
        /*public async Task<IActionResult> LoginUserAsync(Users user)
        {
            throw new NotImplementedException();
        }*/
    }
}
