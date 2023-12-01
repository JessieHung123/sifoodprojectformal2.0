using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0;
using SiFoodProjectFormal2._0.Areas.Users.Models.SPGatewayModels;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
using SiFoodProjectFormal2._0.Models;
using System.Text;

namespace sifoodprojectformal2._0.Areas.Users.Controllers
{
    [Area("Users")]
    public class TransactionController : Controller
    {
        private readonly Sifood3Context _context;
        private readonly IUserIdentityService _userIdentityService;
        public TransactionController(Sifood3Context context, IUserIdentityService userIdentityService)
        {
            _userIdentityService = userIdentityService;
            _context = context;
        }
        private BankInfoModel _bankInfoModel = new BankInfoModel
        {
            MerchantID = "MS150834052",
            HashKey = "EXyVmIxZL7uvX09Sk2wJp5hoDyGPocWn",
            HashIV = "C09KA4wzx60UCOqP",
            NotifyURL = "http://yourWebsitUrl/Bank/SpgatewayNotify",
            ReturnURL = "https://localhost:7042/Users/Home/RealTimeOrders",
            AuthUrl = " https://ccore.newebpay.com/MPG/mpg_gateway",
        };

        public IActionResult CheckOut()
        {
            return View();
        }

        [HttpGet]
        [Route("/Transaction/GetCheckoutData")]
        public IQueryable<CheckOutVM> GetCheckoutData()
        {
            string id = "U002";
            //string userId = _userIdentityService.GetUserId();

            var CheckOutData = _context.Carts.Include(x => x.User).Where(y => y.UserId == id).Select(y => new CheckOutVM
            {
                UserName = y.User.UserName,
                UserAddress = y.User.UserAddresses.Select(y => y.UserDetailAddress).ToList(),
                //UserAddressList = (List<AddressItemVM>)y.User.UserAddresses.Select(x => new AddressItemVM
                //{
                //    UserAdress = x.UserDetailAddress,
                //    AdressIsDefault = x.IsDefault,
                //}),
                ProductId = y.ProductId,
                ProductName = _context.Products.Where(c => c.ProductId == y.ProductId).Select(x => x.ProductName).Single(),
                Quantity = y.Quantity,
                UnitPrice = _context.Products.Where(c => c.ProductId == y.ProductId).Select(x => x.UnitPrice).FirstOrDefault(),
                TotalPrice = y.Quantity * _context.Products.Where(c => c.ProductId == y.ProductId).Select(x => x.UnitPrice).FirstOrDefault(),
                StoreName = _context.Stores.Where(s => s.StoreId == y.Product.StoreId).Select(p => p.StoreName).Single(),
            });
            return CheckOutData;
        }

        [HttpPost]
        [Route("/Transaction/TakeOutOrder")]
        public string TakeOutOrder([FromBody] CreateTakeOutOrderVM model)
        {
            string StoreId = _context.Stores.Where(s => s.StoreName == model.StoreName).Select(s => s.StoreId).Single();
            string UserId = _context.Users.Where(x => x.UserName == model.UserName).Select(x => x.UserId).Single();
            Order order = new Order
            {
                OrderDate = DateTime.Now,
                StoreId = StoreId,
                UserId = UserId,
                DeliveryMethod = "自取",
                StatusId = 1,
                TotalPrice = model.TotalPrice
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var item in model.ProductDetails)
            {
                int productId = GetProductIdByName(item.ProductName);
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = productId,
                    Quantity = item.Quantity,
                };
                _context.OrderDetails.Add(orderDetail);
            }
            _context.SaveChanges();

            return "訂單下訂成功!";

            //string version = "2";
            //// 目前時間轉換 +08:00, 防止傳入時間或Server時間時區不同造成錯誤
            //DateTimeOffset taipeiStandardTimeOffset = DateTimeOffset.Now.ToOffset(new TimeSpan(8, 0, 0));
            //TradeInfo tradeInfo = new TradeInfo()
            //{
            //    // * 商店代號
            //    MerchantID = _bankInfoModel.MerchantID,
            //    // * 回傳格式
            //    RespondType = "String",
            //    // * TimeStamp
            //    TimeStamp = taipeiStandardTimeOffset.ToUnixTimeSeconds().ToString(),
            //    // * 串接程式版本
            //    Version = version,
            //    // * 商店訂單編號
            //    //MerchantOrderNo = $"T{DateTime.Now.ToString("yyyyMMddHHmm")}",
            //    MerchantOrderNo = $"{order.OrderId}",
            //    // * 訂單金額
            //    Amt = model.TotalPrice,
            //    // * 商品資訊
            //    ItemDesc = "商品資訊(自行修改)",
            //    // 支付完成 返回商店網址
            //    ReturnURL = _bankInfoModel.ReturnURL,

            //    NotifyURL = _bankInfoModel.NotifyURL,
            //    // * 付款人電子信箱
            //    Email = string.Empty,
            //    // 付款人電子信箱 是否開放修改(1=可修改 0=不可修改)
            //    EmailModify = 1,
            //    // 信用卡 一次付清啟用(1=啟用、0或者未有此參數=不啟用)
            //    CREDIT = 1,
            //};
            //Atom<string> result = new Atom<string>()
            //{
            //    IsSuccess = true
            //};
            //var inputModel = new SpgatewayInputModel
            //{
            //    MerchantID = _bankInfoModel.MerchantID,
            //    Version = version
            //};
            //// 將model 轉換為List<KeyValuePair<string, string>>, null值不轉
            //List<KeyValuePair<string, string>> tradeData = LambdaUtil.ModelToKeyValuePairList<TradeInfo>(tradeInfo);
            //// 將List<KeyValuePair<string, string>> 轉換為 key1=Value1&key2=Value2&key3=Value3...
            //var tradeQueryPara = string.Join("&", tradeData.Select(x => $"{x.Key}={x.Value}"));
            //// AES 加密
            //inputModel.TradeInfo = CryptoUtil.EncryptAESHex(tradeQueryPara, _bankInfoModel.HashKey, _bankInfoModel.HashIV);
            //// SHA256 加密
            //inputModel.TradeSha = CryptoUtil.EncryptSHA256($"HashKey={_bankInfoModel.HashKey}&{inputModel.TradeInfo}&HashIV={_bankInfoModel.HashIV}");

            //// 將model 轉換為List<KeyValuePair<string, string>>, null值不轉
            //List<KeyValuePair<string, string>> postData = LambdaUtil.ModelToKeyValuePairList<SpgatewayInputModel>(inputModel);

            //Response.Clear();

            //StringBuilder s = new StringBuilder();
            //s.Append("<html>");
            //s.AppendFormat("<body onload='document.forms[\"form\"].submit()'>");
            //s.AppendFormat("<form name='form' action='{0}' method='post'>", _bankInfoModel.AuthUrl);
            //foreach (KeyValuePair<string, string> item in postData)
            //{
            //    s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", item.Key, item.Value);
            //}

            //s.Append("</form></body></html>");
            //Response.WriteAsync(s.ToString());
            //HttpContext.Response.CompleteAsync();

            //return Content(string.Empty);
        }

        private int GetProductIdByName(string productName)
        {
            return _context.Products.Where(p => p.ProductName == productName).Select(p => p.ProductId).FirstOrDefault();
        }

        [HttpPost]
        [Route("/Transaction/DeliverOrder")]
        public string DeliverOrder([FromBody]CreateDeliverOrderVM model)
        {
            string StoreId = _context.Stores.Where(s => s.StoreName == model.StoreName).Select(s => s.StoreId).Single();
            string UserId = _context.Users.Where(x => x.UserName == model.UserName).Select(x => x.UserId).Single();
            Order order = new Order
            {
                OrderDate = DateTime.Now,
                StoreId = StoreId,
                UserId = UserId,
                DeliveryMethod = "外送",
                StatusId = 1,
                TotalPrice = model.TotalPrice,
                ShippingFee = 40,
                Address = model.UserAddress
            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            foreach (var item in model.ProductDetails)
            {
                int productId = GetProductIdByName(item.ProductName);
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = productId,
                    Quantity = item.Quantity,
                };
                _context.OrderDetails.Add(orderDetail);
            }
            _context.SaveChanges();
            return "訂單下訂成功!";
        }

        public IActionResult Payment()
        {
            return View();
        }
    }
}
