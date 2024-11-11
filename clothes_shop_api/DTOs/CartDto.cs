
using clothes_shop_api.DTOs.CartItemDtos;

namespace clothes_shop_api.DTOs
{
    public class CartDto
    {
        public string Id { get; set; }
        public int? UserId { get; set; }
        public List<CartItemDto> CartItems { get; set; }
    }
}
