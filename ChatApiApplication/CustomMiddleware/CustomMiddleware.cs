using ChatApiApplication.Data;
using ChatApiApplication.Model;
using Microsoft.EntityFrameworkCore;

namespace ChatApiApplication.CustomMiddleware
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomMiddleware> _logger;

        public CustomMiddleware(RequestDelegate next, ILogger<CustomMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, ChatAPIDbContext dbContext)
        {

            string token = context.Request.Headers["Authorization"];
            token = (token != null) ? token.Substring("Bearer ".Length).Trim() : "";
            string username = await GetUsernameFromDatabase(dbContext, token);
            Log log = new Log
            {
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                RequestBody = context.Request.ToString(),
                TimeOfCall = DateTime.Now,
                UserName = username
            };
            dbContext.Logs.Add(log);
            await dbContext.SaveChangesAsync();
            await _next(context);
        }

        private async Task<string> GetUsernameFromDatabase(ChatAPIDbContext dbContext, object token)
        {
            var chatUser = await dbContext.ChatUsers.FirstOrDefaultAsync(u => u.AccessToken == token );
            return chatUser != null ? chatUser.UserName : "Anonymous";
        }
    }

    public static class ClassWithNoImplementationMiddleWareExtensions
    {
        public static IApplicationBuilder UseclassWithNoImplementationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomMiddleware>();   
        }
    }
}
