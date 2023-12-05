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
            ViewData["MerchantID"] = Config.GetSection("MerchantID").Value;
            ViewData["ReturnURL"] = $"https://5084-114-34-121-89.ngrok-free.app/Users/Member/RealTimeOrders";//上線換網址記得改
            ViewData["NotifyURL"] = $"https://5084-114-34-121-89.ngrok-free.app/Users/Transaction/CallbackNotify";//上線換網址記得改
            ViewData["ClientBackURL"] = $"https://5084-114-34-121-89.ngrok-free.app/Users/Transaction/Checkout"; //上線換網址記得改
            return View();
        }

        [HttpGet]
        [Route("/Transaction/GetCheckoutData")]
        public IQueryable<CheckOutVM> GetCheckoutData()
        {
            string userId = _userIdentityService.GetUserId();
            var CheckOutData = _context.Carts.Include(x => x.User).Where(y => y.UserId == userId).Select(y => new CheckOutVM
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
        [HttpPost]
        [Route("/Transaction/SendToNewebPay")]
        public IActionResult SendToNewebPay([FromForm] SendToNewebPayIn inModel)
        {
            string orderId = _context.Orders.Include(o => o.User).Where(o => o.User.UserName == inModel.UserName).OrderBy(o => o.OrderId).Select(o => o.OrderId).LastOrDefault().ToString();
            SendToNewebPayOut outModel = new SendToNewebPayOut();
            List<KeyValuePair<string, string>> TradeInfo = new List<KeyValuePair<string, string>>();
            TradeInfo.Add(new KeyValuePair<string, string>("MerchantID", inModel.MerchantID));
            TradeInfo.Add(new KeyValuePair<string, string>("RespondType", "String"));
            TradeInfo.Add(new KeyValuePair<string, string>("TimeStamp", DateTimeOffset.Now.ToOffset(new TimeSpan(8, 0, 0)).ToUnixTimeSeconds().ToString()));
            TradeInfo.Add(new KeyValuePair<string, string>("Version", "2.0"));
            TradeInfo.Add(new KeyValuePair<string, string>("MerchantOrderNo", orderId));
            TradeInfo.Add(new KeyValuePair<string, string>("Amt", inModel.Amt));
            TradeInfo.Add(new KeyValuePair<string, string>("ItemDesc", inModel.ItemDesc));
            TradeInfo.Add(new KeyValuePair<string, string>("ReturnURL", inModel.ReturnURL));
            TradeInfo.Add(new KeyValuePair<string, string>("NotifyURL", inModel.NotifyURL));
            TradeInfo.Add(new KeyValuePair<string, string>("ClientBackURL", inModel.ClientBackURL));
            TradeInfo.Add(new KeyValuePair<string, string>("EmailModify", "1"));
            string TradeInfoParam = string.Join("&", TradeInfo.Select(x => $"{x.Key}={x.Value}"));
            outModel.MerchantID = inModel.MerchantID;
            outModel.Version = "2.0";
            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            string HashKey = Config.GetSection("HashKey").Value;
            string HashIV = Config.GetSection("HashIV").Value;
            string TradeInfoEncrypt = EncryptAESHex(TradeInfoParam, HashKey, HashIV);
            outModel.TradeInfo = TradeInfoEncrypt;
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
        public IActionResult CallbackReturn()
        {
            StringBuilder receive = new StringBuilder();
            foreach (var item in Request.Form)
            {
                receive.AppendLine(item.Key + "=" + item.Value + "<br>");
            }
            ViewData["ReceiveObj"] = receive.ToString();

            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            string HashKey = Config.GetSection("HashKey").Value;
            string HashIV = Config.GetSection("HashIV").Value;

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
        public IActionResult CallbackCustomer()
        {
            StringBuilder receive = new StringBuilder();
            foreach (var item in Request.Form)
            {
                receive.AppendLine(item.Key + "=" + item.Value + "<br>");
            }
            ViewData["ReceiveObj"] = receive.ToString();
            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            string HashKey = Config.GetSection("HashKey").Value;
            string HashIV = Config.GetSection("HashIV").Value;
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
        public IActionResult CallbackNotify()
        {
            StringBuilder receive = new StringBuilder();
            foreach (var item in Request.Form)
            {
                receive.AppendLine(item.Key + "=" + item.Value + "<br>");
            }
            ViewData["ReceiveObj"] = receive.ToString();

            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            string HashKey = Config.GetSection("HashKey").Value;
            string HashIV = Config.GetSection("HashIV").Value;
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
        public string DecryptAESHex(string source, string cryptoKey, string cryptoIV)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(source))
            {
                byte[] sourceBytes = ToByteArray(source);
                if (sourceBytes != null)
                {
                    result = Encoding.UTF8.GetString(DecryptAES(sourceBytes, cryptoKey, cryptoIV)).Trim();
                }
            }
            return result;
        }

        /// <summary>
        /// 將16進位字串轉換為byteArray
        /// </summary>
        /// <param name="source">欲轉換之字串</param>
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
        public static byte[] DecryptAES(byte[] source, string cryptoKey, string cryptoIV)
        {
            byte[] dataKey = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] dataIV = Encoding.UTF8.GetBytes(cryptoIV);
            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
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
        public string DeliverOrder([FromBody] CreateDeliverOrderVM model)
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
    }
}
