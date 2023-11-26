using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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

        [HttpGet]
        public IActionResult JoinUs()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinUs([Bind("StoreId,StoreName,ContactName,TaxId,Email,Phone,City,Region,Address,Description,OpeningTime,ClosingDay,PhotosPath,LogoPath")] JoinUsViewModel joinus)
        {
            if (ModelState.IsValid)
            {

                // 處理 Logo 圖片上傳
                if (Request.Form.Files["LogoPath"] != null)
                {
                    var file = Request.Form.Files["LogoPath"];
                    //存到images/JoinUs
                    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/JoinUs/Logo", file.FileName);

                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // 保存路徑
                    joinus.LogoPath = "/images/JoinUs/Logo/" + file.FileName;
                }


                //處理店家照片上傳
                if (Request.Form.Files["PhotoPath"] != null)
                {
                    var file = Request.Form.Files["PhotoPath"];
                    //存到images/JoinUs
                    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/JoinUs/Photo", file.FileName);

                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // 保存路徑
                    joinus.LogoPath = "/images/JoinUs/Photo/" + file.FileName;
                }

                var store = new Store
                {
                    // 將joinus 的數據映射到 store 實體
                    // 賦值
                    StoreName = joinus.StoreName,
                    ContactName = joinus.ContactName,
                    Email = joinus.Email,
                    Phone = joinus.Phone,
                    TaxId = joinus.TaxId,
                    City = joinus.City,
                    Region = joinus.Region,
                    Address = joinus.Address,
                    Description = joinus.Description,
                    ClosingDay = joinus.ClosingDay,
                    OpeningTime = joinus.OpeningTime,
                    LogoPath = joinus.LogoPath,
                    PhotosPath = joinus.PhotosPath,
                };

                _context.Add(store);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(JoinUs));
            }
            return View(joinus);
        }


        //之前寫的老版本
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult JoinUs([Bind("StoreName, ContactName,TaxID,Email,Phone,Address,Description,OpeningTime,OpeningDay")]JoinUsViewModel joinUsViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        return RedirectToAction("JoinUs");
        //    }
        //   return View(joinUsViewModel);
        //}

        //之前寫的for AJAX版本
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> JoinUs([Bind("StoreName, ContactName,TaxID,Email,Phone,Address,Description,OpeningTime,OpeningDay")] JoinUsViewModel joinUsViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // 創建一個新的 Store 實例
        //        var joinus = new Store
        //        {
        //            StoreName = joinUsViewModel.StoreName,
        //            ContactName = joinUsViewModel.ContactName,
        //            TaxId = joinUsViewModel.TaxId,
        //            Email = joinUsViewModel.Email,
        //            Phone= joinUsViewModel.Phone,
        //            OpeningDay = joinUsViewModel.OpeningDay,
        //            OpeningTime = joinUsViewModel.OpeningTime,
        //            Address = joinUsViewModel.Address,
        //            Description = joinUsViewModel.Description,

        //        };

        //        // 將 joinus 實例添加到數據庫上下文的 Stores 集合中
        //        _context.Stores.Add(joinus);

        //        // 保存更改到數據庫
        //        await _context.SaveChangesAsync();

        //        // 返回 JSON 響應
        //        return Json(new { success = true, message = "申請已成功提交！" });
        //    }
        //    else
        //    {
        //        // 返回包含錯誤信息的 JSON 響應
        //        return Json(new { success = false, message = "表單驗證失敗，請檢查輸入內容。" });
        //    }
        //}



        public IActionResult MemberShip()
        {
            return View();
        }
        public IActionResult Stores()
        {
            return View();
        }
        //public IActionResult JoinUs()
        //{
        //    return View();
        //}



    public IActionResult Products()
        {
            //ViewBag.ProductName=
            //    ViewBag.StoreName =
            //    ViewBag.Address =
            //    ViewBag.Description =
            //    ViewBag.UnitPrice =
            //    ViewBag.PhotoPath =
            //    ViewBag.SuggestPickUpTime =
            //    ViewBag.SuggestPickEndTime =
            //    ViewBag.RemainingStock=
            return View();
        }

        
        public IActionResult RealTimeOrders()
        {
            return View();
        }
       
        
    }
}
 

   