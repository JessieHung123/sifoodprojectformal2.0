using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SiFoodProjectFormal2._0.Models;

namespace SiFoodProjectFormal2._0.Areas.Users.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HistoryOrderapiController : ControllerBase
    {
        private readonly Sifood3Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStoreIdentityService _storeIdentityService;

        public HistoryOrderapiController(Sifood3Context context, IWebHostEnvironment webHostEnvironment, IStoreIdentityService storeIdentityService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _storeIdentityService = storeIdentityService;
        }




    }
}
