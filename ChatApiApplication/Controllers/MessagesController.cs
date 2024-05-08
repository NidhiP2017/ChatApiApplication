using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApiApplication.Controllers
{
    [ApiController]
    [Route("api/")]
    public class MessagesController : Controller
    {
        public ChatAPIDbContext _context;
        public readonly IMessagesService _ims;
        public MessagesController(ChatAPIDbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpPost]
        [Route("messages")]
        public async Task<IActionResult> SendMessage(MessagesDTO sendMsg)
        {
            var sendNewMsg = await _ims.SendMessageAsync(sendMsg);
            return Ok(sendNewMsg);
        }

    }
}
