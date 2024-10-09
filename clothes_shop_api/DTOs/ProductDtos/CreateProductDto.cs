namespace clothes_shop_api.DTOs.ProductDtos
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string? Description { get; set; }
        public int Discount { get; set; }
        public string Slug { get; set; } = null!;
    }
}
