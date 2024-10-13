using System;
using System.Collections.Generic;

namespace clothes_shop_api.Data.Entities
{
    public partial class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
        public bool IsSub { get; set; }
        public int? ProductId { get; set; }
        public string? PublicId { get; set; }

        public virtual Product? Product { get; set; }
    }
}
