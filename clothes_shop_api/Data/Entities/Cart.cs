using System;
using System.Collections.Generic;

namespace clothes_shop_api.Data.Entities
{
    public partial class Cart
    {
        public Cart()
        {
            CartItems = new HashSet<CartItem>();
        }

        public string Id { get; set; } = null!;
        public int? UserId { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
