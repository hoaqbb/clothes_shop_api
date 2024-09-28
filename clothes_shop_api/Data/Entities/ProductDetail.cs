using System;
using System.Collections.Generic;

namespace clothes_shop_api.Data.Entities
{
    public partial class ProductDetail
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int ColorId { get; set; }
        public int QuantityId { get; set; }
    }
}
