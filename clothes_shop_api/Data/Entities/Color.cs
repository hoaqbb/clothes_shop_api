using System;
using System.Collections.Generic;

namespace clothes_shop_api.Data.Entities
{
    public partial class Color
    {
        public Color()
        {
            ProductColors = new HashSet<ProductColor>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ColorCode { get; set; } = null!;

        public virtual ICollection<ProductColor> ProductColors { get; set; }
    }
}
