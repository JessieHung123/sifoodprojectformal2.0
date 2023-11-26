using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SiFoodProjectFormal2._0.Models;
using SiFoodProjectFormal2._0.ViewModels.Users;
using System.Text.Json; 

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

                //處理資料庫string型態問題
                string logoPathInDb = null;
                List<string> photoPathsInDb = new List<string>();


                // 處理 Logo 圖片上傳
                if (joinus.LogoPath != null)
                {
                    var file = joinus.LogoPath;
                    //存到images/JoinUs
                    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/JoinUs/Logo", file.FileName);

                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // 保存路徑
                    logoPathInDb = "/images/JoinUs/Logo/" + file.FileName;
                }


                //處理店家多張照片上傳
                if (joinus.PhotosPath != null && joinus.PhotosPath.Count > 0)
                {
                    foreach (var photo in joinus.PhotosPath)
                    {
                        var photoSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/JoinUs/Photo", photo.FileName);

                        using (var stream = new FileStream(photoSavePath, FileMode.Create))
                        {
                            await photo.CopyToAsync(stream);
                        }

                        photoPathsInDb.Add("/images/JoinUs/Photo/" + photo.FileName);
                    }
                }

                //串接照片路徑
                string concatenatedPhotoPaths = String.Join(",", photoPathsInDb);

                //創建store實體

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
                    LogoPath = logoPathInDb,
                    // 存儲串接後的照片路徑
                    PhotosPath = concatenatedPhotoPaths, 
                };

                _context.Add(store);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "申請已成功提交！" });
            }
            // 如果模型狀態無效，返回 JSON 錯誤信息
            return Json(new { success = false, message = "表單驗證失敗，請檢查輸入內容。" });
 
        }




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
 

   