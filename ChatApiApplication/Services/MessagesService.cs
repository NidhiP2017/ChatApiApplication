using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ChatApiApplication.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly ChatAPIDbContext _appContext;
        private readonly IConfiguration _config;

        public MessagesService(ChatAPIDbContext context, IConfiguration config)
        {
            _appContext = context;
            _config = config;
        }
        public async Task<IActionResult> SendMessageAsync(MessagesDTO msgDTO)
        {
            //Guid senderId = new Guid("a6d2530addc047ee830008dc6f22d88a");
            if (msgDTO != null)
            {
                var newMsg = new Messages()
                {
                    SenderId = msgDTO.SenderId,
                    ReceiverId = msgDTO.ReceiverId,
                    Content = msgDTO.Content,
                    Timestamp = DateTime.Now,
                };
                await _appContext.Messages.AddAsync(newMsg);
                await _appContext.SaveChangesAsync();
                return new OkResult();
            } else {
                return new OkObjectResult("Invalid Content");
            }

        }

        public async Task<IActionResult> EditMessageAsync(Guid msgId, UpdateMsgDTO uMsgDTO)
        {
            if (uMsgDTO != null)
            {
                var msg = await _appContext.Messages.FindAsync(uMsgDTO.MessageId);
                if (msg != null)
                {
                    msg.Content = uMsgDTO.Content;
                    await _appContext.SaveChangesAsync();
                    return new OkObjectResult(msg);
                }
                else
                {
                    return new OkObjectResult("Empty Message");
                }
            }
            else
            {
                return new OkObjectResult("Invalid Content");
            }
        }

        public async Task<IActionResult> DeleteMessageAsync(Guid deleteMsgId)
        {
            if (deleteMsgId != null)
            {
                var msg = await _appContext.Messages.FindAsync(deleteMsgId);
                if (msg != null)
                {
                    _appContext.Remove(msg);
                    await _appContext.SaveChangesAsync();
                    return new OkObjectResult("Message Deleted");
                }
                else
                {
                    return new OkObjectResult("Could Not find message to delete");
                }
            }
            return new OkObjectResult("Blank Message ID");
        }

        /*public Task<IActionResult> RetriveMessageAsync(MessagesDTO msgDTO)
        {
            if(msgDTO != null)
            {
                var query = 
            }
        }*/
    }
    
}
