﻿namespace SiFoodProjectFormal2._0.ViewModels.Users
{
    public class OrderVM
    {
        public string OrderId { get; set; } 

        public DateTime OrderDate { get; set; }

        public string Address { get; set; } = null!;

        public string Status { get; set; }

        public int StatusId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; } = null!;

        public string UserPhone { get; set; } = null!;

        public string? PaymentMethodＮame { get; set; }

        public DateTime PaymentTime { get; set; } 

        public IEnumerable<OrderDetailsVM> OrderDetails { get; set; }

        public string DriverFullName { get; set; } 

        public decimal ShippingFee { get; set; }

        public decimal Subtotal { get; set; }

        public int TotalQuantity { get; set; }

    }
}
