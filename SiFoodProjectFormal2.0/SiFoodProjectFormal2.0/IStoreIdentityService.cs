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
            // 尝试获取指定的 Claim
            Claim? store = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);

            // 如果找到了 Claim，则返回其值；否则返回 null
            return store?.Value;
        }
    }
}
