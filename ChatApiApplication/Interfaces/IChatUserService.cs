using ChatApiApplication.DTO;
using ChatApiApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApiApplication.Interfaces
{
    public interface IChatUserService
    {
        Task<bool> IsEmailUniqueAsync(string email);
        Task<IActionResult> AddUserAsync(RegisterDto usersDTO);
        Task<IActionResult> GetAllUsersAsync(IQueryable<Guid> userId);
        Task<IActionResult> AuthenticateUser(LoginDto usersDTO);
        Task<List<MessagesDTO>> SearchMsgs(string msg);
        Task<IActionResult> UpdateStatus(Guid userId, string status);
        Task<IActionResult> UploadPhoto(List<IFormFile> files);
        public string GetToken();
    }

}
