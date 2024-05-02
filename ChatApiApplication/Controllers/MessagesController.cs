using ChatApiApplication.Data;
using Microsoft.AspNetCore.Mvc;

namespace ChatApiApplication.Controllers
{
    [ApiController]
    [Route("api/")]
    public class MessagesController : Controller
    {
        private readonly ChatAPIDbContext dbContext;
        public MessagesController(ChatAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

    }
}
