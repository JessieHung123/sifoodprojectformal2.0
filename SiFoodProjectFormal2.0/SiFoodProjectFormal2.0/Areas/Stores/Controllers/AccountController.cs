using Microsoft.AspNetCore.Mvc;

namespace sifoodprojectformal2._0.Areas.Stores.Controllers
{
    [Area("Stores")]
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
