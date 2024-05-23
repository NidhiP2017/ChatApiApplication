using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
            _jwtToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            _chatAPIDbContext = chatAPIDbContext;
            _jwtToken = (_jwtToken != null )?_jwtToken.Substring("Bearer ".Length).Trim() : ""; // to remove Bearer string from Token

            var query = from u in _chatAPIDbContext.ChatUsers
                        where u.AccessToken == _jwtToken
                        select u.UserId;
            userId = query.FirstOrDefault();
        }

        //[Authorize]
        [HttpPost]
        [Route("messages")]
        public async Task<IActionResult> SendMessage(SendMessageRequest sendMsg)
        {
            
                //Guid userId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6");
                //sendMsg.SenderId = userId;
                if(sendMsg.SenderId == sendMsg.ReceiverId)
                {
                    return BadRequest("Sender and Receiver Id should be different");
                }

                var sendNewMsg = await _ims.SendMessageAsync(sendMsg);
                var msgDtoResponse = new MessagesDTO()
                {
                    MessageId = sendNewMsg.Value.MessageId,
                    SenderId = sendNewMsg.Value.SenderId,
                    ReceiverId = sendNewMsg.Value.ReceiverId,
                    Content = sendNewMsg.Value.Content, 
                    Timestamp = sendNewMsg.Value.Timestamp,
                };

                return Ok(msgDtoResponse);

        }

        [HttpPut]
        [Route("messages/{msgId}")]
        public async Task<IActionResult> EditMessage(Guid msgId, [Required]
        [StringLength(1000, MinimumLength = 2)]string content)
        {
          //  Guid userId = new Guid("1AC9689F-155C-4E33-C548-08DC73EA4971"); // temporary static sender ID
            var orgMsg =  _ims.GetMessage (msgId);

            if (orgMsg!= null) { 
                var updateMsg = await _ims.EditMessageAsync(msgId, content);
                if (updateMsg != null)
                {
                   
                    return Ok("Message updated successfully.");
                }
                else {
                    return BadRequest("Msg could not be updated");
                }
            } else
            {
                return NotFound("Msg Not found for requested Id");
            }
        }
        [HttpDelete]
        [Route("messages/{msgId}")]

        public async Task<IActionResult> DeleteMsg(Guid msgId)
        {
         //   Guid userId = new Guid("1AC9689F-155C-4E33-C548-08DC73EA4971"); // temporary static sender ID
            var orgMsg = _ims.GetMessage(msgId);
            if(orgMsg!= null) { 
                var deleteMsg = await _ims.DeleteMessageAsync(msgId);
                return Ok("Message Deleted Successfully");
            }
            else
            {
                return NotFound("Msg Not found for requested Id");
            }
        }

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
    }
}
