using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0;
using SiFoodProjectFormal2._0.Areas.Users.Models.NewebPayModels;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
using SiFoodProjectFormal2._0.Models;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Web;
using static SiFoodProjectFormal2._0.Areas.Users.Models.NewebPayModels.PayModels;

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
        public IActionResult CheckOut()
        {
            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            // 產生測試資訊
            ViewData["MerchantID"] = Config.GetSection("MerchantID").Value;
            ViewData["MerchantOrderNo"] = DateTime.Now.ToString("yyyyMMddHHmmss");//訂單編號    
            ViewData["ReturnURL"] = $"https://5084-114-34-121-89.ngrok-free.app/Member/RealTimeOrders"; //支付完成返回商店網址
            ViewData["NotifyURL"] = $"https://5084-114-34-121-89.ngrok-free.app/Transaction/CallbackNotify"; //支付通知網址
            ViewData["ClientBackURL"] = $"https://5084-114-34-121-89.ngrok-free.app/Transaction/Checkout"; //返回商店網址 
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
                UserAddressList = (List<AddressItemVM>)y.User.UserAddresses.Select(x => new AddressItemVM
                {
                    UserAddress = x.UserDetailAddress,
                    AdressIsDefault = x.IsDefault,
                }),
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

            foreach (var Items in model.ProductDetails)
            {
                int productId = GetProductIdByName(Items.ProductName);
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = productId,
                    Quantity = Items.Quantity,
                };
                _context.OrderDetails.Add(orderDetail);
            }
            _context.SaveChanges();
            return "訂單下定成功";
        }

        /// <summary>
        /// 傳送訂單至藍新金流
        /// </summary>
        /// <param name="inModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Transaction/SendToNewebPay")]
        public IActionResult SendToNewebPay([FromForm]SendToNewebPayIn inModel)
        {
            SendToNewebPayOut outModel = new SendToNewebPayOut();

            // 藍新金流線上付款

            //交易欄位
            List<KeyValuePair<string, string>> TradeInfo = new List<KeyValuePair<string, string>>();
            // 商店代號
            TradeInfo.Add(new KeyValuePair<string, string>("MerchantID", inModel.MerchantID));
            // 回傳格式
            TradeInfo.Add(new KeyValuePair<string, string>("RespondType", "String"));
            // TimeStamp
            TradeInfo.Add(new KeyValuePair<string, string>("TimeStamp", DateTimeOffset.Now.ToOffset(new TimeSpan(8, 0, 0)).ToUnixTimeSeconds().ToString()));
            // 串接程式版本
            TradeInfo.Add(new KeyValuePair<string, string>("Version", "2.0"));
            // 商店訂單編號
            TradeInfo.Add(new KeyValuePair<string, string>("MerchantOrderNo", inModel.MerchantOrderNo));
            // 訂單金額
            TradeInfo.Add(new KeyValuePair<string, string>("Amt", inModel.Amt));
            // 商品資訊
            TradeInfo.Add(new KeyValuePair<string, string>("ItemDesc", inModel.ItemDesc));
            // 支付完成返回商店網址
            TradeInfo.Add(new KeyValuePair<string, string>("ReturnURL", inModel.ReturnURL));
            // 支付通知網址
            TradeInfo.Add(new KeyValuePair<string, string>("NotifyURL", inModel.NotifyURL));
            // 支付取消返回商店網址
            TradeInfo.Add(new KeyValuePair<string, string>("ClientBackURL", inModel.ClientBackURL));
            // 付款人電子信箱
            TradeInfo.Add(new KeyValuePair<string, string>("Email", inModel.Email));
            // 付款人電子信箱 是否開放修改(1=可修改 0=不可修改)
            TradeInfo.Add(new KeyValuePair<string, string>("EmailModify", "1"));

            //信用卡 付款
            if (inModel.ChannelID == "CREDIT")
            {
                TradeInfo.Add(new KeyValuePair<string, string>("CREDIT", "1"));
            }
            string TradeInfoParam = string.Join("&", TradeInfo.Select(x => $"{x.Key}={x.Value}"));

            // API 傳送欄位
            // 商店代號
            outModel.MerchantID = inModel.MerchantID;
            // 串接程式版本
            outModel.Version = "2.0";
            //交易資料 AES 加解密
            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            string HashKey = Config.GetSection("HashKey").Value;//API 串接金鑰
            string HashIV = Config.GetSection("HashIV").Value;//API 串接密碼
            string TradeInfoEncrypt = EncryptAESHex(TradeInfoParam, HashKey, HashIV);
            outModel.TradeInfo = TradeInfoEncrypt;
            //交易資料 SHA256 加密
            outModel.TradeSha = EncryptSHA256($"HashKey={HashKey}&{TradeInfoEncrypt}&HashIV={HashIV}");

            return Json(outModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorPayViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// 支付完成返回網址
        /// </summary>
        /// <returns></returns>
        public IActionResult CallbackReturn()
        {
            // 接收參數
            StringBuilder receive = new StringBuilder();
            foreach (var item in Request.Form)
            {
                receive.AppendLine(item.Key + "=" + item.Value + "<br>");
            }
            ViewData["ReceiveObj"] = receive.ToString();

            // 解密訊息
            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            string HashKey = Config.GetSection("HashKey").Value;//API 串接金鑰
            string HashIV = Config.GetSection("HashIV").Value;//API 串接密碼

            string TradeInfoDecrypt = DecryptAESHex(Request.Form["TradeInfo"], HashKey, HashIV);
            NameValueCollection decryptTradeCollection = HttpUtility.ParseQueryString(TradeInfoDecrypt);
            receive.Length = 0;
            foreach (String key in decryptTradeCollection.AllKeys)
            {
                receive.AppendLine(key + "=" + decryptTradeCollection[key] + "<br>");
            }
            ViewData["TradeInfo"] = receive.ToString();

            return View();
        }

        /// <summary>
        /// 商店取號網址
        /// </summary>
        /// <returns></returns>
        public IActionResult CallbackCustomer()
        {
            // 接收參數
            StringBuilder receive = new StringBuilder();
            foreach (var item in Request.Form)
            {
                receive.AppendLine(item.Key + "=" + item.Value + "<br>");
            }
            ViewData["ReceiveObj"] = receive.ToString();

            // 解密訊息
            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            string HashKey = Config.GetSection("HashKey").Value;//API 串接金鑰
            string HashIV = Config.GetSection("HashIV").Value;//API 串接密碼
            string TradeInfoDecrypt = DecryptAESHex(Request.Form["TradeInfo"], HashKey, HashIV);
            NameValueCollection decryptTradeCollection = HttpUtility.ParseQueryString(TradeInfoDecrypt);
            receive.Length = 0;
            foreach (String key in decryptTradeCollection.AllKeys)
            {
                receive.AppendLine(key + "=" + decryptTradeCollection[key] + "<br>");
            }
            ViewData["TradeInfo"] = receive.ToString();
            return View();
        }

        /// <summary>
        /// 支付通知網址
        /// </summary>
        /// <returns></returns>
        public IActionResult CallbackNotify()
        {
            // 接收參數
            StringBuilder receive = new StringBuilder();
            foreach (var item in Request.Form)
            {
                receive.AppendLine(item.Key + "=" + item.Value + "<br>");
            }
            ViewData["ReceiveObj"] = receive.ToString();

            // 解密訊息
            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            string HashKey = Config.GetSection("HashKey").Value;//API 串接金鑰
            string HashIV = Config.GetSection("HashIV").Value;//API 串接密碼
            string TradeInfoDecrypt = DecryptAESHex(Request.Form["TradeInfo"], HashKey, HashIV);
            NameValueCollection decryptTradeCollection = HttpUtility.ParseQueryString(TradeInfoDecrypt);
            receive.Length = 0;
            foreach (String key in decryptTradeCollection.AllKeys)
            {
                receive.AppendLine(key + "=" + decryptTradeCollection[key] + "<br>");
            }
            ViewData["TradeInfo"] = receive.ToString();

            return View();
        }

        /// <summary>
        /// 加密後再轉 16 進制字串
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <param name="cryptoKey">加密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>加密後的字串</returns>
        public string EncryptAESHex(string source, string cryptoKey, string cryptoIV)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                var encryptValue = EncryptAES(Encoding.UTF8.GetBytes(source), cryptoKey, cryptoIV);

                if (encryptValue != null)
                {
                    result = BitConverter.ToString(encryptValue)?.Replace("-", string.Empty)?.ToLower();
                }
            }

            return result;
        }

        /// <summary>
        /// 字串加密AES
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <param name="cryptoKey">加密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>加密後字串</returns>
        public byte[] EncryptAES(byte[] source, string cryptoKey, string cryptoIV)
        {
            byte[] dataKey = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] dataIV = Encoding.UTF8.GetBytes(cryptoIV);

            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                aes.Key = dataKey;
                aes.IV = dataIV;

                using (var encryptor = aes.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(source, 0, source.Length);
                }
            }
        }

        /// <summary>
        /// 字串加密SHA256
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <returns>加密後字串</returns>
        public string EncryptSHA256(string source)
        {
            string result = string.Empty;

            using (System.Security.Cryptography.SHA256 algorithm = System.Security.Cryptography.SHA256.Create())
            {
                var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(source));

                if (hash != null)
                {
                    result = BitConverter.ToString(hash)?.Replace("-", string.Empty)?.ToUpper();
                }

            }
            return result;
        }

        /// <summary>
        /// 16 進制字串解密
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <param name="cryptoKey">加密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>解密後的字串</returns>
        public string DecryptAESHex(string source, string cryptoKey, string cryptoIV)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                // 將 16 進制字串 轉為 byte[] 後
                byte[] sourceBytes = ToByteArray(source);

                if (sourceBytes != null)
                {
                    // 使用金鑰解密後，轉回 加密前 value
                    result = Encoding.UTF8.GetString(DecryptAES(sourceBytes, cryptoKey, cryptoIV)).Trim();
                }
            }

            return result;
        }

        /// <summary>
        /// 將16進位字串轉換為byteArray
        /// </summary>
        /// <param name="source">欲轉換之字串</param>
        /// <returns></returns>
        public byte[] ToByteArray(string source)
        {
            byte[] result = null;

            if (!string.IsNullOrWhiteSpace(source))
            {
                var outputLength = source.Length / 2;
                var output = new byte[outputLength];

                for (var i = 0; i < outputLength; i++)
                {
                    output[i] = Convert.ToByte(source.Substring(i * 2, 2), 16);
                }
                result = output;
            }

            return result;
        }

        /// <summary>
        /// 字串解密AES
        /// </summary>
        /// <param name="source">解密前字串</param>
        /// <param name="cryptoKey">解密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>解密後字串</returns>
        public static byte[] DecryptAES(byte[] source, string cryptoKey, string cryptoIV)
        {
            byte[] dataKey = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] dataIV = Encoding.UTF8.GetBytes(cryptoIV);

            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                // 智付通無法直接用PaddingMode.PKCS7，會跳"填補無效，而且無法移除。"
                // 所以改為PaddingMode.None並搭配RemovePKCS7Padding
                aes.Padding = System.Security.Cryptography.PaddingMode.None;
                aes.Key = dataKey;
                aes.IV = dataIV;

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] data = decryptor.TransformFinalBlock(source, 0, source.Length);
                    int iLength = data[data.Length - 1];
                    var output = new byte[data.Length - iLength];
                    Buffer.BlockCopy(data, 0, output, 0, output.Length);
                    return output;
                }
            }
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
