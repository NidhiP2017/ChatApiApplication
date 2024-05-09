using ChatApiApplication.Data;
using System.Text;

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
            string ipAddress = context.Connection.RemoteIpAddress.ToString();

            string requestBody = context.Request.ToString();

            DateTime requestTime = DateTime.Now;

            string username = context.User.Identity.Name ?? "Anonymous";

            _logger.LogInformation($"Request from IP: {ipAddress}, Time: {requestTime}, User: {username}, Body: {requestBody}");

            await _next(context);
        }

        /*private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();
            var body = request.Body;

            using (var reader = new StreamReader(body, Encoding.UTF8, true, 1024, true))
            {
                var requestBody = await reader.ReadToEndAsync();
                body.Seek(0, SeekOrigin.Begin);
                request.Body = body;
                return requestBody;
            }
        }*/
    }

    public static class ClassWithNoImplementationMiddleWareExtensions
    {
        public static IApplicationBuilder UseclassWithNoImplementationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomMiddleware>();   
        }
    }
}
