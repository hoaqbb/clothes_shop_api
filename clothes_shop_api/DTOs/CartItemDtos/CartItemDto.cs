using clothes_shop_api.DTOs.QuantityDtos;

namespace clothes_shop_api.DTOs.CartItemDtos
{
    public class CartItemDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Photo { get; set; }
        public int Quantity { get; set; }
        public int Discount { get; set; }
        public string Category { get; set; }
        public string Slug { get; set; }
        public QuantityDto ProductVariant { get; set; }
    }
}
