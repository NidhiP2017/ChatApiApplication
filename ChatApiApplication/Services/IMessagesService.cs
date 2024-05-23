using ChatApiApplication.DTO;
using ChatApiApplication.Model;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ChatApiApplication.Services
{
    public interface IMessagesService
    {
        Task<ActionResult<MessagesDTO>> SendMessageAsync(SendMessageRequest msgDTO);
        Task<IActionResult> EditMessageAsync(Guid msgId, string content);
        Task<IActionResult> DeleteMessageAsync(Guid msgId);
        Messages GetMessage(Guid msgId);
        Task<IActionResult> RetriveConversationHistoryAsync(Guid userId, DateTime? before, int count, string sort);
        //Task<IActionResult> RetriveMessageAsync(MessagesDTO msgDTO);
    }
}
