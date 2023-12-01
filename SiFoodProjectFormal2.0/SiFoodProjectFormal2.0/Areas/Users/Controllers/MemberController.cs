using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
using SiFoodProjectFormal2._0.Areas.Stores.ViewModels;
using System.Data.SqlTypes;
using System.Text;
using System.Security.Cryptography;

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


        //11/23新版
        [HttpGet]
        [Route("/Member/Profile")]
        public async Task<IActionResult> Profile()
        {
            //if (id == null || _context.Users == null)
            //{
            //    return NotFound();
            //}

            var loginuserId = "U001";
            //先測試寫死ID，之後要改成取當前登入者的資料
            var user = await _context.Users.Where(u => u.UserId == loginuserId).SingleAsync();

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
        [Route("/Member/Profile")]
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
        [Route("/Member/ChangePassword")]
        public string ChangePassword([FromBody]UserPwdChange model)
        {
            User? user = _context.Users.Where(x => x.UserEmail == model.UserEmail).FirstOrDefault();

            SHA256 sha256 = SHA256.Create();
            byte[] passwordBytes = Encoding.ASCII.GetBytes($"{model?.OldPassword}{user?.UserPasswordSalt}");
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);

            if (passwordBytes != user?.UserPasswordHash)
            {
                return "新密碼與舊密碼不符";
            }
            else
            {
                byte[] saltBytes = new byte[8];
                using (RandomNumberGenerator ran = RandomNumberGenerator.Create())
                {
                    ran.GetBytes(saltBytes);
                }
                user.UserPasswordSalt = saltBytes;
                byte[] NewPasswordBytes = Encoding.ASCII.GetBytes($"{model?.NewPassword}{saltBytes}");
                byte[] NewHashBytes = sha256.ComputeHash(NewPasswordBytes);
                user.UserPasswordHash = NewHashBytes;
                _context.SaveChanges();
                return "密碼修改成功";
            }
        }

        public IActionResult Products()
        {
            return View();
        }
        public IActionResult RealTimeOrders()
        {
            return View();
        }


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

        [Route("/Member/Favorite")]
        public async Task<IActionResult> Favorite()
        {
            // var userId = "當前用戶的ID"; // 從用戶會話或身份驗證系統獲取
            //暫時先寫死
            var userId = "U001";

            //加入計算收藏幾間店家的功能
            var favoriteStoresCount = _context.Favorites.Count(f => f.UserId == userId);
            ViewBag.FavoriteStoresCount = favoriteStoresCount;

            // 查詢收藏的商家
            var favoriteStores = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Store)
                .Select(f => new FavoriteVM
                {
                    FavoriteId = f.FavoriteId,
                    StoreName = f.Store.StoreName,
                    LogoPath = f.Store.LogoPath
                })
                .ToListAsync();

            return View(favoriteStores);
        }



        [HttpPost]
        public IActionResult DeleteFavorite(int[] selectedFavorites, int? singleDelete)
        {
            // 如果單獨刪除被觸發，只將 singleDelete 值加入 selectedFavorites
            if (singleDelete.HasValue)
            {
                selectedFavorites = new int[] { singleDelete.Value };
            }

            // 假設 currentUserId 是當前用戶的 UserId
            // 從用戶身份驗證系統獲取,現在先暫時指定鈺晴首頁使用的ID
            string currentUserId = "U001";

            // 根據 selectedFavorites 刪除收藏項目
            foreach (var favoriteId in selectedFavorites)
            {
                var favorite = _context.Favorites
                    .FirstOrDefault(f => f.UserId == currentUserId && f.FavoriteId == favoriteId);

                if (favorite != null)
                {
                    _context.Favorites.Remove(favorite);
                }
            }
            _context.SaveChanges();

            return RedirectToAction("Favorite");
        }

        [Route("/Member/Address")]
        public IActionResult Address()
        {
            return View();
        }
    }
}