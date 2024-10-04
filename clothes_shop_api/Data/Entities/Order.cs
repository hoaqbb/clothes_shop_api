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
        public decimal Total { get; set; }
        public byte Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int UserId { get; set; }
        public int PaymentId { get; set; }

        public virtual Payment Payment { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
