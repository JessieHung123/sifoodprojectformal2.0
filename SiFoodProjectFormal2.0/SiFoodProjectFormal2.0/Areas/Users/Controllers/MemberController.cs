using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Models;
using SiFoodProjectFormal2._0.ViewModels.Users;
using SiFoodProjectFormal2._0.Helper;

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

        //[HttpGet]
        //public IActionResult Profile()
        //{
        //    return View();
        //}

        //Develop測試版:先從db拿第一個User
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // 從資料庫獲取第一筆用戶記錄
            var user = await _context.Users.FirstOrDefaultAsync();

            if (user != null)
            {
                // 創建 ViewModel 並填充資料
                var viewModel = new ProfileViewModel
                {
                    UserName = user.UserName,
                    UserEmail = user.UserEmail,
                    UserPhone = user.UserPhone,
                    UserBirthDate = user.UserBirthDate
                    // 根據需要填充其他欄位
                };

                return View(viewModel);
            }

            // 如果找不到用戶，處理錯誤情況
            return RedirectToAction("ErrorPage"); // 或其他適當的錯誤處理方式
        }
        //測試版end


        //舊版
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile([Bind("UserName,UserEmail,UserPhone,UserBirthDate")] ProfileViewModel profileViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(profileViewModel);
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == profileViewModel.UserEmail);

            // 判斷用戶是否存在並執行相應操作
            if (existingUser != null)
            {
                // 更新現有用戶資料
                existingUser.UserName = profileViewModel.UserName;
                existingUser.UserPhone = profileViewModel.UserPhone;
                existingUser.UserBirthDate = profileViewModel.UserBirthDate;

                _context.Users.Update(existingUser);
            }
            else
            {
                // 創建新用戶
                var newUser = new User
                {
                    UserName = profileViewModel.UserName,
                    UserEmail = profileViewModel.UserEmail,
                    UserPhone = profileViewModel.UserPhone,
                    UserBirthDate = profileViewModel.UserBirthDate
                };

                _context.Users.Add(newUser);
            }

            //保存更改
            await _context.SaveChangesAsync();

            // 設置 TempData 成功消息
            TempData["SuccessMessage"] = "您的個人資料已更新成功！";

            //返回重導向到 Profile 頁面
            return RedirectToAction("Profile");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ProfileViewModel model)
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
            bool validPassword = PasswordHelper.VerifyPassword(model.CurrentPassword, user.UserPasswordHash, user.UserPasswordSalt);
            if (!validPassword)
            {
                ModelState.AddModelError("CurrentPassword", "當前密碼不正確");
                return View("Profile", model);
            }

            // 更新用戶的密碼
            (user.UserPasswordHash, user.UserPasswordSalt) = PasswordHelper.CreatePasswordHash(model.NewPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // 密碼更新成功後，你可能想要重導向用戶到個人資料頁面，或顯示一個成功消息
            TempData["SuccessMessage"] = "密碼更新成功";
            return RedirectToAction("Profile");
        }




        public IActionResult Products()
        {
            return View();
        }
        public IActionResult RealTimeOrders()
        {
            return View();
        }
   

public IActionResult Favorite()
        {
            return View();
        }
        public IActionResult HistoryOrders()
        {
            return View();
        }
        public IActionResult Address()
        {
            return View();
        }
    }
}
