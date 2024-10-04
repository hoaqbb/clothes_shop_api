using clothes_shop_api.Data.Entities;

namespace clothes_shop_api.DTOs.QuantityDtos
{
    public class QuantityDto
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public string? Color { get; set; }
        public string? ColorCode { get; set; }
        public string Size { get; set; } = null!;
    }
}
