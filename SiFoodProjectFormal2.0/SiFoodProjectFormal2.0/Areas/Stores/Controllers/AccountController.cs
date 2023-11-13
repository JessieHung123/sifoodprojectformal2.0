using Microsoft.AspNetCore.Mvc;

namespace sifoodprojectformal.Areas.StoresPlateform.Controllers
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
