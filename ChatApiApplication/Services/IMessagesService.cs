using ChatApiApplication.DTO;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ChatApiApplication.Services
{
    public interface IMessagesService
    {
        Task<IActionResult> SendMessageAsync(MessagesDTO msgDTO);
        Task<IActionResult> EditMessageAsync(Guid msgId, UpdateMsgDTO uMsgDTO);
        Task<IActionResult> DeleteMessageAsync(Guid msgId);
        //Task<IActionResult> RetriveMessageAsync(MessagesDTO msgDTO);
    }
}
