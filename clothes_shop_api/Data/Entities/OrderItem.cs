using System;
using System.Collections.Generic;

namespace clothes_shop_api.Data.Entities
{
    public partial class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductDetailId { get; set; }
        public int Quantity { get; set; }
    }
}
