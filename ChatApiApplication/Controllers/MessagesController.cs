using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ChatApiApplication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/")]
    public class MessagesController : Controller
    {
        public readonly IMessagesService _ims;
        private string _jwtToken;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ChatAPIDbContext _chatAPIDbContext;
        public Guid userId;

        public MessagesController(IHttpContextAccessor httpContextAccessor,  IMessagesService ims, ChatAPIDbContext chatAPIDbContext)
        {
            _ims = ims;
            _httpContextAccessor = httpContextAccessor;
            _chatAPIDbContext = chatAPIDbContext;
           
        }

        [Authorize]
        [HttpPost("/getAllMyMessages")]
        public async Task<IActionResult> getAllMyMessages()
        {
            var messages = await _ims.getAllMyMessages();
            return Ok(messages);
        }

        [Authorize]
        [HttpPost]
        [Route("messages")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest sendMsg)
        {
            var sendNewMsg = await _ims.SendMessageAsync(sendMsg);
            return Ok(sendNewMsg);
        }
        [Authorize]
        [HttpPut]
        [Route("editMessages/{msgId}")]
        public async Task<IActionResult> EditMessage(Guid msgId, [Required]
        [StringLength(1000, MinimumLength = 2)]string content)
        {
            var orgMsg =  _ims.GetMessage (msgId);
            if (orgMsg != null)
            {
                var updateMsg = await _ims.EditMessageAsync(msgId, content);
                return Ok(updateMsg);
            }
            else
            {
                return NotFound("Msg Not found for requested Id");
            }
        }
        [Authorize]
        [HttpDelete]
        [Route("deleteMessages/{msgId}")]

        public async Task<IActionResult> DeleteMsg(Guid msgId)
        {
            var orgMsg = _ims.GetMessage(msgId);
            if (orgMsg != null)
            {
                var deleteMsg = await _ims.DeleteMessageAsync(msgId);
                return Ok(deleteMsg);
            }
            else
            {
                return NotFound("Msg Not found for requested Id");
            }
        }
        [Authorize]
        [HttpGet]
        [Route("messages")]
        public async Task<IActionResult> RetriveConversationHistoryAsync([FromQuery] Guid userId,
            [FromQuery] DateTime? before = null,
            [FromQuery] int count = 20,
            [FromQuery] string sort = "asc")
        {

            var messages = await _ims.RetriveConversationHistoryAsync(userId, before, count, sort);
            return Ok(messages);
        }

        [Authorize]
        [HttpPost("{messageId}/replyInThread")]
        /*reply to a particular message in thread*/
        public async Task<IActionResult> ReplyToMsg(Guid messageId, [FromBody] ThreadMessageDTO messageRequest)
        {
            return await _ims.ReplyToMsg(null, messageId, messageRequest);
        }
    }
}
