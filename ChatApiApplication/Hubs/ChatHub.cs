using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApiApplication.Hubs
{
    //[Authorize]
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string connectionId = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKV1RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiI5YTZiYWI1Zi1hN2VkLTRjOTktOWJkMy1iZDcxYTIwNWRmZTciLCJpYXQiOiIxNC0wNS0yMDI0IDA4OjIzOjM5IiwiZXhwIjoxNzE2NTM5MDE5LCJpc3MiOiJKV1RBdXRoZW50aWNhdGlvblNlcnZlciIsImF1ZCI6IkpXVFNlcnZpY2VQb3N0bWFuQ2xpZW50In0.toOyyedkk-qo55sVqXBwEoy50PKxNLp1aUGNaR316eg";
            await Clients.All.SendAsync("recieved message", $"{Context.ConnectionId} has joined");
        }

        public async Task SendMessage(string userId, string message)
        {
            userId = "1ac9689f-155c-4e33-c548-08dc73ea4971";
            await Clients.User(userId).SendAsync("ReceiveMessage", message);
        }
    }
}
