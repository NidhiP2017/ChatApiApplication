using ChatApiApplication.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApiApplication.Controllers
{
    [ApiController]
    [Route("api/sendmessages")]
    public class ChatMsgController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatMsgController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        /*[HttpPost]
        public async Task<IActionResult> SendMessage(string receiverId, string message)
        {
            await _hubContext.Clients.User(receiverId).SendAsync("ReceiveMessage", message);
            return Ok();
        }*/
    }
}
