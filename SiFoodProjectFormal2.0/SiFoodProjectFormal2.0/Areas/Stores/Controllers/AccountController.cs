using Microsoft.AspNetCore.Mvc;

namespace sifoodprojectformal.Areas.Stores.Controllers
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
