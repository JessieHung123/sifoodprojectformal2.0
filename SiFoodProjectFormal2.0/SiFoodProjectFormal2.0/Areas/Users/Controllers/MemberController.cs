using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Models;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;

namespace sifoodprojectformal2._0.Areas.Users.Controllers
{
    [Area("Users")]
    public class MemberController : Controller
    {
        Sifood3Context _context;

        public MemberController(Sifood3Context context)
        {
            _context = context;
        }

        //原版
        //[HttpGet]
        //public IActionResult Profile()
        //{
        //    return View();
        //}

        //Develop測試版:先從db拿第一個User
        //[HttpGet]
        //public async Task<IActionResult> Profile()
        //{
        //    // 從資料庫獲取第一筆用戶記錄
        //    var user = await _context.Users.FirstOrDefaultAsync();

        //    if (user != null)
        //    {
        //        // 創建 ViewModel 並填充資料
        //        var viewModel = new ProfileViewModel
        //        {
        //            UserName = user.UserName,
        //            UserEmail = user.UserEmail,
        //            UserPhone = user.UserPhone,
        //            UserBirthDate = user.UserBirthDate
        //            // 根據需要填充其他欄位
        //        };

        //        return View(viewModel);
        //    }

        //    // 如果找不到用戶，處理錯誤情況
        //    return RedirectToAction("ErrorPage"); // 或其他適當的錯誤處理方式
        //}
        ////測試版end


        ////舊版
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Profile([Bind("UserName,UserEmail,UserPhone,UserBirthDate")] ProfileViewModel profileViewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(profileViewModel);
        //    }

        //    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == profileViewModel.UserEmail);

        //    // 判斷用戶是否存在並執行相應操作
        //    if (existingUser != null)
        //    {
        //        // 更新現有用戶資料
        //        existingUser.UserName = profileViewModel.UserName;
        //        existingUser.UserPhone = profileViewModel.UserPhone;
        //        existingUser.UserBirthDate = profileViewModel.UserBirthDate;

        //        _context.Users.Update(existingUser);
        //    }
        //    else
        //    {
        //        // 創建新用戶
        //        var newUser = new User
        //        {
        //            UserName = profileViewModel.UserName,
        //            UserEmail = profileViewModel.UserEmail,
        //            UserPhone = profileViewModel.UserPhone,
        //            UserBirthDate = profileViewModel.UserBirthDate
        //        };

        //        _context.Users.Add(newUser);
        //    }

        //    //保存更改
        //    await _context.SaveChangesAsync();

        //    // 設置 TempData 成功消息
        //    TempData["SuccessMessage"] = "您的個人資料已更新成功！";

        //    //返回重導向到 Profile 頁面
        //    return RedirectToAction("Profile");
        //}

        [Route("/Member/Profile")]
        //11/23新版
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            //if (id == null || _context.Users == null)
            //{
            //    return NotFound();
            //}

            var loginuserId = "U002";
            //先測試寫死ID，之後要改成取當前登入者的資料
            var user = await _context.Users.Where(u=>u.UserId== loginuserId).SingleAsync();

            if (user != null)
            {
                // 創建 ViewModel 並填充資料
                var viewModel = new ProfileVM
                {
                    //填充欄位資料
                    UserName = user.UserName,
                    UserEmail = user.UserEmail,
                    UserPhone = user.UserPhone,
                    UserBirthDate = user.UserBirthDate
                };

                return View(viewModel);
            }

            // 如果找不到用戶，處理錯誤情況
            return RedirectToAction("ErrorPage"); // 或其他適當的錯誤處理方式
        }

        
        //11/23新版
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(string id, [Bind("UserName,UserEmail,UserPhone,UserBirthDate")] ProfileVM profileViewModel)
        {
            //if (ModelState.IsValid)
            //{

            var loginuserId = "U002";
            var userToUpdate = await _context.Users.FindAsync(loginuserId);
                if (userToUpdate == null)
                {
                    return NotFound();
                }

                // 更新用戶數據
                userToUpdate.UserName = profileViewModel.UserName;
                userToUpdate.UserEmail = profileViewModel.UserEmail;
                userToUpdate.UserPhone = profileViewModel.UserPhone;
                userToUpdate.UserBirthDate = profileViewModel.UserBirthDate;

                // 其他必要的更新操作

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Profile));
            

            return View(profileViewModel);
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Route("/Member/ChangePassword")]
        public async Task<IActionResult> ChangePassword(ProfileVM model)
        {
            // 驗證模型
            if (!ModelState.IsValid)
            {
                // 處理模型驗證錯誤
                return View("Profile", model);
            }

            // 取得當前用戶
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == User.Identity.Name);

            if (user == null)
            {
                // 處理找不到用戶的情況
                ModelState.AddModelError("", "無法找到用戶資料。");
                return View("Profile", model);
            }

            // 驗證當前密碼
            //bool validPassword = PasswordHelper.VerifyPassword(model.CurrentPassword, user.UserPasswordHash, user.UserPasswordSalt);
            //if (!validPassword)
            //{
            //    ModelState.AddModelError("CurrentPassword", "當前密碼不正確");
            //    return View("Profile", model);
            //}

            // 更新用戶的密碼
            //(user.UserPasswordHash, user.UserPasswordSalt) = PasswordHelper.CreatePasswordHash(model.NewPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // 密碼更新成功後，你可能想要重導向用戶到個人資料頁面，或顯示一個成功消息
            TempData["SuccessMessage"] = "密碼更新成功";
            return RedirectToAction("Profile");
        }

        [Route("/Member/Favorite")]
        public IActionResult Favorite()
        {
            return View();
        }
        [Route("/Member/HistoryOrders")]
        public IActionResult HistoryOrders(string searchTerm = null, int pageSize = 20)
        {
            IQueryable<Order> historyOrdersQuery = _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Status);

                //關鍵字搜尋
                if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                historyOrdersQuery = historyOrdersQuery.Where(o =>
                    o.OrderDetails.Any(od => od.Product.ProductName.Contains(searchTerm)));
            }

            // 計算總訂單數
            var totalOrdersCount = historyOrdersQuery.Count();

             // 將總訂單數傳遞給視圖
            ViewBag.TotalOrdersCount = totalOrdersCount;

            //下拉控制顯示筆數
            var historyOrders = historyOrdersQuery

            // 使用 pageSize 來限制返回的結果數量
            .Take(pageSize) 
            .Select(o => new HistoryOrderVM
            {
                    StoreId = o.StoreId,
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    Status = o.Status.StatusName,
                    Quantity = o.OrderDetails.Sum(od => od.Quantity),
                    TotalPrice = o.TotalPrice ?? 0,
                    FirstProductPhotoPath = o.OrderDetails.FirstOrDefault().Product.PhotoPath,
                    FirstProductName = o.OrderDetails.FirstOrDefault().Product.ProductName
                }).ToList();

            return View(historyOrders);
        }
        [Route("/Member/Address")]
        public IActionResult Address()
        {
            return View();
        }
    }
}
