using System.ComponentModel.DataAnnotations;

namespace clothes_shop_api.DTOs.QuantityDtos
{
    public class UpdateQuantityDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Amount { get; set; }
    }
}
