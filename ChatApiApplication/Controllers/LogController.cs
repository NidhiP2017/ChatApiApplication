using ChatApiApplication.Data;
using ChatApiApplication.Model;
using ChatApiApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApiApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _jwtToken;
        private readonly ChatAPIDbContext _context;
        public LogController(IHttpContextAccessor httpContextAccessor, ChatAPIDbContext chatAPIDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            _context = chatAPIDbContext;
            _jwtToken = (_jwtToken != null) ? _jwtToken.Substring("Bearer ".Length).Trim() : "";
        }

        [HttpGet]
        public IActionResult GetLogs(DateTime? startTime = null, DateTime? endTime = null)
        {
            IQueryable<Log> query = _context.Logs;

            DateTime defaultStartTime = DateTime.UtcNow.AddMinutes(-5);
            DateTime defaultEndTime = DateTime.UtcNow;

            startTime = startTime != null ? startTime : defaultStartTime;
            endTime = endTime != null ? endTime : defaultEndTime;

            query = query.Where(log => log.TimeOfCall >= startTime.Value && log.TimeOfCall <= endTime.Value);
            
            var logs = query.ToList();

            return Ok(logs);
        }
    }

    public class LogRequestParameters
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
