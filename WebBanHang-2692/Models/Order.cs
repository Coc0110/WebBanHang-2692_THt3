using System;
using System.Collections.Generic;

namespace WebBanHang_2692.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Liên kết với Identity User
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        public string ShippingAddress { get; set; }
        public string Notes { get; set; }

        public ApplicationUser User { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public string ReceiverName { get; set; }
        public string PhoneNumber { get; set; }
    }
}