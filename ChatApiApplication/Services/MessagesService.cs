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

        public IActionResult GetMessage(Guid userId, Guid msgId)
        {
            var query = _appContext.Messages.FirstOrDefault(u => u.SenderId == userId && u.MessageId == msgId);
            if (query == null)
            {
                return null ;
            }

            return (IActionResult)query;
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
        
        public async Task<IActionResult> RetriveConversationHistoryAsync(Guid userId, DateTime? before, int count, string sort)
        {
            //if (_jwtToken != null)
            //{
            var query = _appContext.Messages.Where(m => m.SenderId == userId);
            if (before.HasValue)
            {
                query = query.Where(m => m.Timestamp < before.Value);
            }
            query = sort.ToLower() == "asc" ? query.OrderBy(m => m.Timestamp) : query.OrderByDescending(m => m.Timestamp);
            query = query.Take(count);
            var messages = await query.ToListAsync();
            return new OkObjectResult(messages);
            /* }
             else
             {
                 return BadRequest("Could not Authorize");
             }*/
        }
    }
    
}
