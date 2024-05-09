using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ChatApiApplication.Controllers
{
    [ApiController]
    [Route("api/")]
    public class MessagesController : Controller
    {
        public readonly IMessagesService _ims;

        public MessagesController(IMessagesService ims)
        {
            _ims = ims;
        }

        [HttpPost]
        [Route("messages")]
        public async Task<IActionResult> SendMessage(MessagesDTO sendMsg)
        {
            if (sendMsg != null)
            {
                var sendNewMsg = await _ims.SendMessageAsync(sendMsg);
                var msgDtoResponse = new MessagesDTO()
                {
                    MessageId = sendMsg.MessageId,
                    SenderId = sendMsg.SenderId,
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

        }
        [HttpDelete]
        [Route("messages/{deleteMsgId:guid}")]

        public async Task<IActionResult> DeleteMsg(Guid msgId)
        {
            var deleteMsg = await _ims.DeleteMessageAsync(msgId);
            return Ok("Message Deleted");
        }
    }
}
