using System;
using System.Collections.Generic;

namespace clothes_shop_api.Data.Entities
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public string? Note { get; set; }
        public decimal Amount { get; set; }
        public byte Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserId { get; set; }
        public int? PaymentId { get; set; }
        public string Address { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public byte Shipping { get; set; }
        public string? Email { get; set; }

        public virtual Payment? Payment { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
