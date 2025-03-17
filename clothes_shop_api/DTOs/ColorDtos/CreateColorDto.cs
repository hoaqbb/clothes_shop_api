using System.ComponentModel.DataAnnotations;

namespace clothes_shop_api.DTOs.ColorDtos
{
    public class CreateColorDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ColorCode { get; set; }
    }
}
