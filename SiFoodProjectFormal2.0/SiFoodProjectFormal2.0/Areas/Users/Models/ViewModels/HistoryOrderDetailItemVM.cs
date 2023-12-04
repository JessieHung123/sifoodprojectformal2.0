﻿namespace SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels
{
    internal class HistoryOrderDetailItemVM
    {
        public string PhotoPath { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => UnitPrice * Quantity;
    }
}