using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;

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
    }
    
}
