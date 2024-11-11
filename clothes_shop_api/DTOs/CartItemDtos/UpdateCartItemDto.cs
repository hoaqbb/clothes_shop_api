namespace clothes_shop_api.DTOs.CartItemDtos
{
    public class UpdateCartItemDto
    {
        public string CartId { get; set; }
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
