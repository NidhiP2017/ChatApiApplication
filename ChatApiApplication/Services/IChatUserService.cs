using ChatApiApplication.DTO;
using ChatApiApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApiApplication.Services
{
    public interface IChatUserService
    {
        Task<bool> IsEmailUniqueAsync(string email);
        Task<IActionResult> AddUserAsync(ChatUsersDTO usersDTO);
        Task<IActionResult> GetAllUsersAsync(string userId);
        Task<IActionResult> AuthenticateUser(ChatUserLoginDTO usersDTO);

        public string GetToken();

    }

}
