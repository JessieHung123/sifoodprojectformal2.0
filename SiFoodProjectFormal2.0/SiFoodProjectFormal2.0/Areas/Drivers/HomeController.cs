using Microsoft.AspNetCore.Mvc;

namespace sifoodprojectformal.Areas.DriversPlatform
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
