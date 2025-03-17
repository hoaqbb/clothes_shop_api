namespace clothes_shop_api.DTOs.ColorDtos
{
    public class ColorListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ColorCode { get; set; } = null!;
        public int ProductCount { get; set; }
    }
}
