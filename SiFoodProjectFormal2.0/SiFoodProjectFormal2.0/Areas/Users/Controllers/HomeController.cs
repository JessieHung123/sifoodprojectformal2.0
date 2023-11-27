﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<string> JoinUsSubmit([Bind("StoreId,StoreName,ContactName,TaxId,Email,Phone,City,Region,Address,Description,OpeningTime,ClosingDay,PhotosPath,PhotosPath2,PhotosPath3,LogoPath")] JoinUsViewModel joinus)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 處理 Logo 圖片上傳
                    string logoPathInDb = await SavePhoto(joinus.LogoPath, "Logo");

                    // 處理三張店家照片上傳
                    string photoPathInDb = await SavePhoto(joinus.PhotosPath, "PhotosPath");
                    string photoPath2InDb = await SavePhoto(joinus.PhotosPath2, "PhotosPath2");
                    string photoPath3InDb = await SavePhoto(joinus.PhotosPath3, "PhotosPath3");

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
                        PhotosPath = photoPathInDb,
                        PhotosPath2 = photoPath2InDb,
                        PhotosPath3 = photoPath3InDb,
                    };

                    _context.Add(store);
                    await _context.SaveChangesAsync();
                    return "成功";

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"錯誤: {ex.Message}");
                }

                }
            // 如果模型狀態無效，返回 JSON 錯誤信息
            return "失敗";
 
        }


        //儲存照片專用方法
        private async Task<string> SavePhoto(IFormFile photo, string folderName)
        {
            if (photo != null)
            {
                var fileName = Path.GetFileName(photo.FileName);
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/JoinUs", folderName, fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                return $"/images/JoinUs/{folderName}/{fileName}";
            }

            return null;
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
 

   