using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Exceptions;
using ChatApiApplication.Interfaces;
using ChatApiApplication.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ChatApiApplication.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly ChatAPIDbContext _appContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly static connection<string> _connections = new connection<string>();
        private readonly GroupService _groupService;
        // private readonly IHubContext<ChatHub1> _hubContext;

        public MessagesService(ChatAPIDbContext context, IConfiguration config,
             IHttpContextAccessor httpContextAccessor)
        {
            _appContext = context;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            //_groupService = groupService;
        }

        public async Task<Guid> GetCurrentLoggedInUser()
        {
            var Id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid currentUserId = await _appContext.ChatUsers.Where(m => m.Id == Id).Select(u => u.UserId).FirstOrDefaultAsync();
            if (currentUserId == null)
                return Guid.Empty;
            else
                return currentUserId;
        }
        public async Task<ActionResult<MessagesDTO>> SendMessageAsync(SendMessageRequest msgDTO)
        {
            var Id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) ? .Value;
            Guid currentUserId = await _appContext.ChatUsers.Where(m => m.Id == Id).Select(u => u.UserId).FirstOrDefaultAsync();
            var checkSenderId = await _appContext.ChatUsers.FindAsync(msgDTO.SenderId);
            var checkReceiverId = await _appContext.ChatUsers.FindAsync(msgDTO.ReceiverId);

            if (checkSenderId == null)
            {
                throw new InvalidOperationException("Unauthorized");
            }
            if (msgDTO == null || string.IsNullOrWhiteSpace(msgDTO.Content))
            {
                return new BadRequestObjectResult(new { Message = "Message content is required" });
            }
            if (checkReceiverId == null)
            {
                return new NotFoundObjectResult(new { Message = "Receiver Not Found" });
            }
            if (checkSenderId == checkReceiverId)
            {
                return new BadRequestObjectResult(new { Message = "Can not send a message to yourself." });
            }
            if (msgDTO != null)
            {
                var newMsg = new Messages()
                {
                    SenderId = currentUserId,
                    ReceiverId = msgDTO.ReceiverId,
                    Content = msgDTO.Content,
                    Timestamp = DateTime.Now,
                };
                await _appContext.Messages.AddAsync(newMsg);
                await _appContext.SaveChangesAsync();

                /*foreach (var connectionId in _connections.GetConnections(newMsg.ReceiverId.ToString()))
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("BroadCast", newMsg);
                }*/
                var response = new MessagesDTO()
                {
                    MessageId = newMsg.MessageId,
                    SenderId = msgDTO.SenderId,
                    ReceiverId = msgDTO.ReceiverId,
                    Content = msgDTO.Content,
                    Timestamp = newMsg.Timestamp
                };
                return new OkObjectResult(response);
            }
            else
            {
                return new OkObjectResult("Invalid Content");
            }
        }
        [Authorize]
        public Messages GetMessage( Guid msgId)
        {
            var query = _appContext.Messages.FirstOrDefault(u => u.MessageId == msgId);
            if (query == null)
            {
                return null;
            }
            return query;
        }
        public async Task<IActionResult> EditMessageAsync(Guid msgId, string content)
        {
            if (content != null)
            {
                var Id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Guid currentUserId = await _appContext.ChatUsers.Where(m => m.Id == Id).Select(u => u.UserId).FirstOrDefaultAsync();
                var msg = await _appContext.Messages.FindAsync(msgId);

                if (msg == null || (msg.SenderId != currentUserId && msg.ReceiverId != currentUserId))
                {
                    return new OkObjectResult("Invalid access or empty message");
                }
                else
                {
                    msg.Content = content;
                    await _appContext.SaveChangesAsync();
                    return new OkObjectResult(msg);
                }
            }
            else
            {
                return new OkObjectResult("Invalid Content");
            }
        }

        public async Task<IActionResult> DeleteMessageAsync(Guid deleteMsgId)
        {
            var Id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid currentUserId = await _appContext.ChatUsers.Where(m => m.Id == Id).Select(u => u.UserId).FirstOrDefaultAsync();
            if (deleteMsgId != null)
            {
                var msg = await _appContext.Messages.FindAsync(deleteMsgId);
                if(msg.SenderId != currentUserId)
                {
                    return new OkObjectResult("You cannot delete others msg");
                }
                if (msg != null )
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
            var query = _appContext.Messages.Where(m => m.SenderId == userId || m.ReceiverId == userId);
            if (before.HasValue)
            {
                query = query.Where(m => m.Timestamp <= before.Value);
            }
            query = sort.ToLower() == "asc" ? query.OrderBy(m => m.Timestamp) : query.OrderByDescending(m => m.Timestamp);
            query = query.Take(count);
            var messages = await query.ToListAsync();
            return new OkObjectResult(messages);
            
        }

        public async Task<IActionResult> ReplyToMsg(int? groupId, Guid messageId, ThreadMessageDTO messageRequest)
        {
            var parentMsg = GetMessage(messageId);
            var currentUserId = await GetCurrentLoggedInUser();
            
            if (string.IsNullOrEmpty(messageRequest.Content))
            {
                throw new ArgumentException("Message is required.");
            }
            else
            {
                var message = new Messages
                {
                    Content = messageRequest.Content,
                    SenderId = currentUserId,
                    ReceiverId = parentMsg.ReceiverId,
                    ParentMessageId = parentMsg.MessageId,
                    GroupId = (groupId != null)?groupId: null,
                    Timestamp = DateTime.Now,
                };
                await _appContext.Messages.AddAsync(message);
                await _appContext.SaveChangesAsync();

                var response = new
                {
                    senderId = currentUserId,
                    content = messageRequest.Content,
                    GroupId = (groupId != null) ? groupId : null,
                    timestamp = message.Timestamp
                };
                return new OkObjectResult(response);
            }
        }
    }

}
