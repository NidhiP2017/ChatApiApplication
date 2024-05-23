using System.Security.Claims;

namespace ChatApiApplication.Services
{
    public class TenantContext : ITenantContext
    {
        public long UserId { get; private set; }
        public void SetValues(IEnumerable<Claim> claims)
        {
            var userId = claims.Where(c => c.Type == "Userid").Select(c => c.Value).SingleOrDefault();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                UserId = Convert.ToInt32(userId);
            }
        }
    }

    public interface ITenantContext
    {
        long UserId { get; }

        void SetValues(IEnumerable<Claim> claims);
    }
}
