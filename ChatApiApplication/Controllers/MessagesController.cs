using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApiApplication.Controllers
{
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
            _jwtToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            _chatAPIDbContext = chatAPIDbContext;
            _jwtToken = (_jwtToken != null )?_jwtToken.Substring("Bearer ".Length).Trim() : ""; // to remove Bearer string from Token

            var query = from u in _chatAPIDbContext.ChatUsers
                        where u.AccessToken == _jwtToken
                        select u.UserId;
            userId = query.FirstOrDefault();

        }

        [Authorize]
        [HttpPost]
        [Route("messages")]
        public async Task<IActionResult> SendMessage(MessagesDTO sendMsg)
        {
            if (sendMsg != null && _jwtToken != null )
            {
                sendMsg.SenderId = userId;
                var sendNewMsg = await _ims.SendMessageAsync(sendMsg);
                var msgDtoResponse = new MessagesDTO()
                {
                    MessageId = sendMsg.MessageId,
                    SenderId = userId,
                    ReceiverId = sendMsg.ReceiverId,
                    Content = sendMsg.Content, 
                    Timestamp = sendMsg.Timestamp,
                };

                return Ok(msgDtoResponse);

            }
            else
            {
                return BadRequest("Please add message content to send");
            }
        }

        [HttpPut]
        [Route("messages/{updateMsgId:guid}")]
        public async Task<IActionResult> EditMessage(Guid msgId, UpdateMsgDTO uMsgDTO)
        {
            Guid userId = new Guid("f211ffcc-c369-42ee-79b7-08dc70aa19d8"); // temporary static sender ID
            var orgMsg =  _ims.GetMessage(userId, msgId);

            if (orgMsg!= null) { 
                var updateMsg = await _ims.EditMessageAsync(msgId, uMsgDTO);
                if (updateMsg != null)
                {
                    var newMsg = new MessagesDTO()
                    {
                        MessageId = uMsgDTO.MessageId,
                        Content = uMsgDTO.Content
                    };
                    return Ok(updateMsg);
                }
                else {
                    return BadRequest("Msg could not be updated");
                }
            } else
            {
                return Unauthorized("Msg cannot be updated of other user");
            }
        }
        [HttpDelete]
        [Route("messages/{deleteMsgId:guid}")]

        public async Task<IActionResult> DeleteMsg(Guid msgId)
        {
            Guid userId = new Guid("f211ffcc-c369-42ee-79b7-08dc70aa19d8"); // temporary static sender ID
            var orgMsg = _ims.GetMessage(userId, msgId);
            if(orgMsg!= null) { 
                var deleteMsg = await _ims.DeleteMessageAsync(msgId);
                return Ok("Message Deleted");
            }
            else
            {
                return Unauthorized("Msg cannot be deleted of other user");
            }
        }

        [HttpGet]
        [Route("messages/{userId:guid}")]
        public async Task<IActionResult> RetriveConversationHistoryAsync([FromQuery] Guid userId,
            [FromQuery] DateTime? before = null,
            [FromQuery] int count = 20,
            [FromQuery] string sort = "asc")
        {

            var messages = await _ims.RetriveConversationHistoryAsync(userId, before, count, sort);
            return Ok(messages);
        }
    }
}
