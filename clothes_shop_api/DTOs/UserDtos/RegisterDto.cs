using System.ComponentModel.DataAnnotations;

namespace clothes_shop_api.DTOs.UserDtos
{
    public class RegisterDto
    {
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public byte Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
