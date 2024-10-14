using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.CategoryDtos;
using clothes_shop_api.DTOs.ProductImageDtos;
using clothes_shop_api.DTOs.QuantityDtos;

namespace clothes_shop_api.DTOs.ProductDtos
{
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public string? Description { get; set; }
        public int? Discount { get; set; }
        public string Slug { get; set; } = null!;
        public CategoryDto Category { get; set; }
        public ICollection<ProductImageDto> ProductImages { get; set; }
        public ICollection<QuantityDto> ProductVariants { get; set; }
    }
}
