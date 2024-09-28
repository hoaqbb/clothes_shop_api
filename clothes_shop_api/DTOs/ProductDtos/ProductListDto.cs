using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.ColorDtos;

namespace clothes_shop_api.DTOs.ProductDtos
{
    public class ProductListDto
    {
        //public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public string MainPhoto { get; set; }
        public string SubPhoto { get; set; }
        public ICollection<ColorDto> ProductColors { get; set; }
        public int? Discount { get; set; }
        public string Slug { get; set; } = null!;
        public string Category { get; set; }
    }
}
