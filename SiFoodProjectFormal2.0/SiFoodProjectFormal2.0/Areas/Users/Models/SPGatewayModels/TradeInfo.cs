namespace SiFoodProjectFormal2._0.Areas.Users.Models.SPGatewayModels
{
    public class TradeInfo
    {
        public string? MerchantID { get; set; }

        /// <summary>
        /// * 回傳格式
        /// <para>JSON 或是 String</para>
        /// </summary>
        public string? RespondType { get; set; }

        /// <summary>
        /// * TimeStamp
        /// <para>自從 Unix 纪元（格林威治時間 1970 年 1 月 1 日 00:00:00）到當前時間的秒數。</para>
        /// </summary>
        public string? TimeStamp { get; set; }

        /// <summary>
        /// * 串接程式版本
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// * 商店訂單編號
        /// <para>限英、數字、_ 格式</para>
        /// <para>長度限制為20字</para>
        /// <para>同一商店中此編號不可重覆。</para>
        /// </summary>
        public string? MerchantOrderNo { get; set; }

        /// <summary>
        /// * 訂單金額
        /// </summary>
        public int Amt { get; set; }

        /// <summary>
        /// * 商品資訊
        /// <para>1.限制長度為50字。</para>
        /// <para>2.編碼為Utf-8格式。</para>
        /// <para>3.請勿使用斷行符號、單引號等特殊符號，避免無法顯示完整付款頁面。</para>
        /// <para>4.若使用特殊符號，系統將自動過濾。</para>
        /// </summary>
        public string? ItemDesc { get; set; }

        public string? NotifyURL { get; set; }

        public string? ReturnURL { get; set; }

        public string? Email { get; set; }

        public int EmailModify { get; set; }

        public int? CREDIT { get; set; }
    }
}
