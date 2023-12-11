using System.Security.Claims;

namespace SiFoodProjectFormal2._0
{
    public interface IStoreIdentityService
    {
        string GetStoreId();
    }
    public class StoreIdentityService : IStoreIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;


        public StoreIdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetStoreId()
        {
            Claim? store = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            return store?.Value;
        }
    }
}
