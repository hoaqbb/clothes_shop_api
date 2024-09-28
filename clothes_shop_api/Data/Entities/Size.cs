using System;
using System.Collections.Generic;

namespace clothes_shop_api.Data.Entities
{
    public partial class Size
    {
        public Size()
        {
            Quantities = new HashSet<Quantity>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Quantity> Quantities { get; set; }
    }
}
