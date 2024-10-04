namespace clothes_shop_api.DTOs.CartDtos
{
    public class CreateCartItemDto
    {
        public int UserId { get; set; }
        public int QuantityId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
