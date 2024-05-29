using ChatApiApplication.Data;
using Microsoft.AspNetCore.SignalR;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ChatApiApplication.Hubs
{
    //[Authorize]
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly ChatAPIDbContext _chatAPIDbContext;
        public ChatHub(IHttpContextAccessor httpContextAccessor, ChatAPIDbContext chatAPIDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _chatAPIDbContext = chatAPIDbContext;
        }
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joineddddddddd");
        }

        public async Task SendToUser()
        {
            Guid userId = new Guid("1AC9689F-155C-4E33-C548-08DC73EA4971");
            await Clients.Client("k72N4KB2cTdVNets1j4QCQ==").SendAsync("ReceiveMsg", userId, "Hiii from SendToUser");
        }

        private async Task<string> GetUserId()
        {
            var query = Context.GetHttpContext().Request.Query;
            var token = query["access_token"];

            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Missing access_token in query string");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new InvalidOperationException("User ID claim not found in JWT token");
            }
            var userId = userIdClaim.Value;
            return userId;
        }

        public async Task SendMessage(string username, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId}:{message}");
            //await Clients.All.SendAsync("ReceiveMessage", username, message);
        }

      
    }
}
