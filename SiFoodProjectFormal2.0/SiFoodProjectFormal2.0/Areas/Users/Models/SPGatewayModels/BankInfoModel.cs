namespace SiFoodProjectFormal2._0.Areas.Users.Models.SPGatewayModels
{
    public class BankInfoModel
    {
        // 商店代號
        public string? MerchantID { get; set; }

        //AES 加密/SHA256 加密 Key
        //</summary>
        public string? HashKey { get; set; }


        // AES 加密/SHA256 加密 IV
        public string? HashIV { get; set; }


        //支付完成 返回商店網址
        //<para>1.交易完成後，以 Form Post 方式導回商店頁面。</para>
        //<para>2.若為空值，交易完成後，消費者將停留在智付通付款或取號完成頁面。</para>
        //<para>3.只接受80與443 Port。</para>
        public string? ReturnURL { get; set; }

        public string? AuthUrl { get; set; }
    }
}
