using System.ComponentModel.DataAnnotations;

namespace clothes_shop_api.DTOs.ColorDtos
{
    public class ColorDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ColorCode { get; set; }
    }
}
