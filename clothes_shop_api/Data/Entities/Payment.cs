using System;
using System.Collections.Generic;

namespace clothes_shop_api.Data.Entities
{
    public partial class Payment
    {
        public Payment()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Method { get; set; } = null!;
        public string? Provider { get; set; }
        public bool Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? TransactionId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; }
    }
}
