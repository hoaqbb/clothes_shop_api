using System.ComponentModel.DataAnnotations;

namespace clothes_shop_api.DTOs.CategoryDtos
{
    public class CreateCategoryDto
    {
        [Required]
        public string Name { get; set; }
    }
}
