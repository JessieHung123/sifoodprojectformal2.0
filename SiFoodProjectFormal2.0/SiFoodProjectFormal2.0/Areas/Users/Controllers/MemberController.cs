using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
using SiFoodProjectFormal2._0.Areas.Stores.ViewModels;
using System.Data.SqlTypes;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Routing;
using SiFoodProjectFormal2._0;

namespace sifoodprojectformal2._0.Areas.Users.Controllers
{
    [Area("Users")]
    public class MemberController : Controller
    {
        Sifood3Context _context;
        private readonly IUserIdentityService _userIdentityService;

        public MemberController(Sifood3Context context, IUserIdentityService userIdentityService)
        {
            _context = context;
            _userIdentityService = userIdentityService;
        }


        //11/23新版
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            //if (id == null || _context.Users == null)
            //{
            //    return NotFound();
            //}

            // var loginuserId = "U002";
            var loginuserId = _userIdentityService.GetUserId();

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
                ViewBag.ID = loginuserId;

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
            // 取得當前用戶
            
            var userToUpdate = await _context.Users.FindAsync(id);
            if (userToUpdate == null)
            {
                // 處理找不到用戶的情況
                ModelState.AddModelError("", "無法找到用戶資料。");
                return RedirectToAction("Main", "Home");
            }

            // 更新用戶數據
            userToUpdate.UserName = profileViewModel.UserName;
            userToUpdate.UserEmail = profileViewModel.UserEmail;
            userToUpdate.UserPhone = profileViewModel.UserPhone;
            userToUpdate.UserBirthDate = profileViewModel.UserBirthDate;

            // 其他必要的更新操作

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Profile));

        }

        [HttpPost]
        [Route("/Member/ChangePassword")]
        public string ChangePassword([FromBody]UserPwdChange model) 
        {
            User? user = _context.Users.Where(x => x.UserEmail == model.UserEmail).FirstOrDefault();

            SHA256 sha256 = SHA256.Create();
            byte[] passwordBytes = Encoding.ASCII.GetBytes($"{model?.OldPassword}{user?.UserPasswordSalt}");
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);

            if (user != null)
            {
                if (Enumerable.SequenceEqual(hashBytes, user.UserPasswordHash))
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
                else
                {
                    return "新密碼與舊密碼不符";
                }
            }

            return "找不到此使用者";

        }

        public IActionResult Products()
        {
            return View();
        }
        public IActionResult RealTimeOrders()
        {
            return View();
        }

        //=========歷史訂單========//
        public IActionResult HistoryOrders(string searchTerm = null, string sortOption = "Status" ,int pageSize = 20)
        {
            // 假定的用戶ID，之後需要替換為當前登入用戶的ID
            var loginuserId = "U001";

            IQueryable<Order> historyOrdersQuery = _context.Orders
                // 添加這行以過濾該用戶的訂單
                .Where(o => o.UserId == loginuserId)

            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Include(o => o.Status);

            // 應用關鍵字過濾
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                historyOrdersQuery = historyOrdersQuery.Where(o =>
                    o.OrderDetails.Any(od => od.Product.ProductName.Contains(searchTerm)));
            }
                        
            //保持搜尋關鍵字在搜尋欄
            ViewBag.SearchTerm = searchTerm;

            //計算總訂單數
            var totalOrdersCount = historyOrdersQuery.Count();
            ViewBag.TotalOrdersCount = totalOrdersCount;


            // Sort排序
            switch (sortOption)
            {
                case "Status":
                    historyOrdersQuery = historyOrdersQuery.OrderBy(o => o.Status.StatusName);
                    break;
                case "Low to High":
                    historyOrdersQuery = historyOrdersQuery.OrderBy(o => o.TotalPrice);
                    break;
                case "High to Low":
                    historyOrdersQuery = historyOrdersQuery.OrderByDescending(o => o.TotalPrice);
                    break;
                case "Newest":
                    historyOrdersQuery = historyOrdersQuery.OrderByDescending(o => o.OrderDate);
                    break;
                case "Oldest":
                    historyOrdersQuery = historyOrdersQuery.OrderBy(o => o.OrderDate);
                    break;
                default:
                    // 默認排序：按訂購日期由新到舊排序
                    historyOrdersQuery = historyOrdersQuery.OrderByDescending(o => o.OrderDate);
                    break;
            }

            // 在過濾後的結果上應用分頁
            var historyOrders = historyOrdersQuery
                .Take(pageSize)
                .Select(o => new HistoryOrderVM
                {
                    // ViewModel的初始化
                    StoreId = o.StoreId,
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    Status = o.Status.StatusName,
                    Quantity = o.OrderDetails.Sum(od => od.Quantity),
                    TotalPrice = Convert.ToInt32(_context.OrderDetails
                                                    .Where(od => od.OrderId == o.OrderId)
                                                    .Sum(od => od.Quantity * od.Product.UnitPrice) +
                                                    o.ShippingFee),
                    FirstProductPhotoPath = o.OrderDetails.FirstOrDefault().Product.PhotoPath,
                    FirstProductName = o.OrderDetails.FirstOrDefault().Product.ProductName
                }).ToList();

            return View(historyOrders);
        }

        //訂單明細方法GetOrderDetails
        public async Task<IActionResult> GetOrderDetails(string orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Comment) // 假設 Order 包含多個 Comment
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var historyOrderDetailsVM = new HistoryOrderDetailVM
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                ShippingFee = order.ShippingFee,
                DeliveryMethod = order.DeliveryMethod,

                Items = order.OrderDetails.Select(od => new HistoryOrderDetailItemVM
                {
                    PhotoPath = od.Product.PhotoPath,
                    ProductName = od.Product.ProductName,
                    UnitPrice = Convert.ToInt32(od.Product.UnitPrice),
                    Quantity = od.Quantity
                }).ToList(),

                CommentRank = order.Comment.CommentRank, 
                CommentContents = order.Comment.Contents 
            };

            return PartialView("_OrderDetailPartial", historyOrderDetailsVM);
        }


        //送出評論
        [HttpPost]
        public async Task<IActionResult> SubmitRating(string orderId, int rating, string comment)
        {
            // 查找訂單
            var order = await _context.Orders.Include(o => o.Comment).FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            // 處理評價和評論
            if (order.Comment == null)
            {
                order.Comment = new Comment { CommentRank = (short)rating, Contents = comment };
            }
            else
            {
                order.Comment.CommentRank = (short)rating;
                order.Comment.Contents = comment;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "評價提交成功" });
        }



        //=========收藏店家========//  
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


        public IActionResult Address()
        {
            return View();
        }
    }
}