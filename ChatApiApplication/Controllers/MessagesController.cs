﻿using ChatApiApplication.Data;
using Microsoft.AspNetCore.Mvc;

namespace ChatApiApplication.Controllers
{
    [ApiController]
    [Route("api/")]
    public class MessagesController : Controller
    {
        public ChatAPIDbContext _context;
        public MessagesController(ChatAPIDbContext dbContext)
        {
            _context = dbContext;
        }

    }
}
