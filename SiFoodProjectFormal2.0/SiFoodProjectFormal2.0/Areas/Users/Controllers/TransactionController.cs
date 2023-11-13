using Microsoft.AspNetCore.Mvc;

namespace sifoodprojectformal2._0.Areas.Users.Controllers
{
    [Area("Users")]
    public class TransactionController : Controller
    {
        public IActionResult Checkout()
        {
            return View();
        }
        public IActionResult Payment()
        {
            return View();
        }
    }
}
