using System;
using System.Collections.Generic;

namespace clothes_shop_api.Data.Entities
{
    public partial class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? QuantityId { get; set; }
        public int Quantity { get; set; }
        public int? CartId { get; set; }

        public virtual Cart? Cart { get; set; }
        public virtual Order Order { get; set; } = null!;
        public virtual Quantity? QuantityNavigation { get; set; }
    }
}
