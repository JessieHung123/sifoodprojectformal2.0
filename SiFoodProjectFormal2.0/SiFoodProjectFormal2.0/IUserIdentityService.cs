using System.Security.Claims;

namespace SiFoodProjectFormal2._0
{
    public interface IUserIdentityService
    {
        string GetUserId();
    }

    public class UserIdentityService : IUserIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserIdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            Claim? user = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            if (user != null)
            {
                return ClaimTypes.Name;
            }
            throw new InvalidOperationException("找不到 User ID");
        }

    }
}

