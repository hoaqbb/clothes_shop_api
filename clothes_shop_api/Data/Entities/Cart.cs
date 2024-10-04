using System;
using System.Collections.Generic;

namespace clothes_shop_api.Data.Entities
{
    public partial class Cart
    {
        public int Id { get; set; }
        public int? Quantity { get; set; }
        public int QuantityId { get; set; }
        public int UserId { get; set; }

        public virtual Quantity QuantityNavigation { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
