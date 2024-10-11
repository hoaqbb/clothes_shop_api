using clothes_shop_api.DTOs.OrderDtos;

namespace clothes_shop_api.DTOs.UserDtos
{
    public class UserDetailDto
    {
        public string Lastname { get; set; } = null!;
        public string Firstname { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = null!;
        public byte Gender { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
