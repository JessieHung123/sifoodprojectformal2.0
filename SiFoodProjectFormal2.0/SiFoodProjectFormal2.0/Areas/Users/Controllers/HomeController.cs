using Microsoft.AspNetCore.Mvc;
using SiFoodProjectFormal2._0.Models;
using SiFoodProjectFormal2._0.ViewModels.Users;

namespace sifoodprojectformal2._0.Areas.Users.Controllers
{
    [Area("Users")]
    public class HomeController : Controller
    {
        Sifood3Context _context;

        public HomeController(Sifood3Context context)
        {
            _context = context;
        }

        public IActionResult Main()
        {
            return View();
        }
        public IActionResult MapFind()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();

        }
        public IActionResult MemberShip()
        {
            return View();
        }
        public IActionResult Stores()
        {
            return View();
        }
        public IActionResult JoinUs()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]

        ////Address>now only one input
        //public IActionResult JoinUs([Bind("StoreName, ContactName,TaxID,Email,Phone,Address,Description,OpeningTime,OpeningDay")]JoinUsViewModel joinUsViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        return RedirectToAction("JoinUs");
        //    }
        //   return View(joinUsViewModel);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinUs([Bind("StoreName, ContactName,TaxID,Email,Phone,Address,Description,OpeningTime,OpeningDay")] JoinUsViewModel joinUsViewModel)
        {
            if (ModelState.IsValid)
            {
                // 創建一個新的 Store 實例
                var store = new Store
                {
                    StoreName = joinUsViewModel.StoreName,
                    ContactName = joinUsViewModel.ContactName,
                    TaxId = joinUsViewModel.TaxId,
                    Email = joinUsViewModel.Email,
                    Phone= joinUsViewModel.Phone,
                    OpeningDay = joinUsViewModel.OpeningDay,
                    OpeningTime = joinUsViewModel.OpeningTime,
                    Address = joinUsViewModel.Address,
                    Description = joinUsViewModel.Description,

                };

                // 將 store 實例添加到數據庫上下文的 Stores 集合中
                _context.Stores.Add(store);

                // 保存更改到數據庫
                await _context.SaveChangesAsync();

                // 返回 JSON 響應
                return Json(new { success = true, message = "申請已成功提交！" });
            }
            else
            {
                // 返回包含錯誤信息的 JSON 響應
                return Json(new { success = false, message = "表單驗證失敗，請檢查輸入內容。" });
            }
        }

        // 其他動作方法...
    public IActionResult Products()
        {
            return View();
        }
        public IActionResult RealTimeOrders()
        {
            return View();
        }
    }
}
 

   