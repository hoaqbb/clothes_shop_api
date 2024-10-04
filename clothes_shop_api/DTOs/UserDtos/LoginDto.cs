using System.ComponentModel.DataAnnotations;

namespace clothes_shop_api.DTOs.UserDtos
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
