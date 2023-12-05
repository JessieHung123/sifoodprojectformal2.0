﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
using SiFoodProjectFormal2._0.Models;
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
        [HttpGet]

        public IActionResult UserFAQ()
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
        public async Task<string> JoinUsSubmit([Bind("StoreId,StoreName,ContactName,TaxId,Email,Phone,City,Region,Address,Description,OpeningTime,ClosingDay,PhotosPath,PhotosPath2,PhotosPath3,LogoPath")] JoinUsVM joinus)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 處理 Logo 圖片上傳
                    string logoPathInDb = await SavePhoto(joinus.LogoPath, "logo");

                    // 處理三張店家照片上傳
                    string photoPathInDb = await SavePhoto(joinus.PhotosPath, "photo");
                    string photoPath2InDb = await SavePhoto(joinus.PhotosPath2, "photo");
                    string photoPath3InDb = await SavePhoto(joinus.PhotosPath3, "photo");

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
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Stores", folderName, fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                return $"/images/Stores/{folderName}/{fileName}";
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

        [Route("/Users/Home/Products/{ProductId?}")]
        public IActionResult Products(int ProductId)
        {
            var IdToString = ProductId.ToString();
            List<string> ProductList = GetCookieProductList();//讀取
            if (ProductList.Contains(IdToString)) { ProductList.Remove(IdToString); }
            if (IdToString != "0") { ProductList.Add(IdToString); }
            SetCookieProductList(ProductList);//取陣列最後一個以外的值
            //倒敘且只留最後五個
            ProductList.Reverse();
            ProductList = ProductList.Take(5).ToList();

            ViewBag.ProductList = ProductList;
            List<ProductVM> cookieProduct = new List<ProductVM>();
            
            if (ProductList != null) { 
                foreach (var productid in ProductList) {
                    var c = _context.Products.Where(p => p.ProductId == int.Parse(productid));
                    ProductVM VM = new ProductVM
                    {
                        ProductId=c.Select(p => p.ProductId).Single(), 
                        ProductName = c.Select(p => p.ProductName).Single(),
                        StoreName = c.Include(p => p.Store).Select(p => p.Store.StoreName).Single(),
                        PhotoPath = c.Select(p => p.PhotoPath).Single(),
                        UnitPrice = Math.Round(c.Select(p => p.UnitPrice).Single(), 2)
                    };
                    cookieProduct.Add(VM);
                }
                ViewBag.CookieProduct = cookieProduct; 
            }
            return View();
        }
        private List<string> GetCookieProductList()
        {
            string? ProductCookieValue = Request.Cookies["Records"];
            List<string> ProductList = new List<string>();
            if (ProductCookieValue != null && ProductCookieValue!="")
            {
                ProductList.AddRange(ProductCookieValue.Split(',')); ;
            }
            return ProductList;//{"32","33","34"}
        }
        private void SetCookieProductList(List<string> product)
        {
            string RecentBrowse = string.Join(",", product);
            CookieOptions CO = new CookieOptions();
            CO.Expires = DateTime.Now.AddDays(1);
            CO.HttpOnly = true;
            CO.Secure = true;
            Response.Cookies.Append("Records", RecentBrowse, CO);
        }
        [HttpGet]
        public IActionResult UserRealTimeOrders()
        {
            return View();
        }
       
        
    }
}
 

   