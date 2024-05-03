using ChatApiApplication.DTO;
using ChatApiApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApiApplication.Services
{
    public interface IUserService
    {
        Task<bool> IsEmailUniqueAsync(string email);
        Task<IActionResult> AddUserAsync(UsersDTO usersDTO);
        //Task LoginUser(Users user);        
    }
}
