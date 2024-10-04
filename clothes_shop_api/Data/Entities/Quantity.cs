using System;
using System.Collections.Generic;

namespace clothes_shop_api.Data.Entities
{
    public partial class Quantity
    {
        public Quantity()
        {
            Carts = new HashSet<Cart>();
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public int Amount { get; set; }
        public int? ProductColorId { get; set; }
        public int SizeId { get; set; }
        public int ProductId { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual ProductColor? ProductColor { get; set; }
        public virtual Size Size { get; set; } = null!;
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
