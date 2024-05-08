using ChatApiApplication.DTO;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ChatApiApplication.Services
{
    public interface IMessagesService
    {
        Task<IActionResult> SendMessageAsync(MessagesDTO msgDTO);
    }
}
