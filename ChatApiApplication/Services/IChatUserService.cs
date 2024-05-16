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
        Task<IActionResult> GetAllUsersAsync(IQueryable<Guid> userId);
        Task<IActionResult> AuthenticateUser(ChatUserLoginDTO usersDTO);

        Task<List<MessagesDTO>> SearchMsgs(string msg);
        public string GetToken();
    }

}
