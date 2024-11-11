using clothes_shop_api.DTOs.ColorDtos;
using clothes_shop_api.DTOs.QuantityDtos;

namespace clothes_shop_api.DTOs.CartDtos
{
    public class CartDto2
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Photo { get; set; }
        public int Quantity { get; set; }
        public int Discount { get; set; }
        public string Category { get; set; }
        //public ColorDto Color { get; set; }
        //public string Size { get; set; }
        public string Slug { get; set; }
        public QuantityDto ProductVariant { get; set; }
    }
}
