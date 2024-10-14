using clothes_shop_api.DTOs.ProductImageDtos;

namespace clothes_shop_api.DTOs.ProductDtos
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public string? Description { get; set; }
        public int Discount { get; set; }
        public string Slug { get; set; } = null!;
        public int? CategoryId { get; set; }
        public int[] ProductColors { get; set; }
        public int[] ProductSizes { get; set; }
        //public bool? IsVisible { get; set; }
    }
}
