namespace clothes_shop_api.DTOs.CartItemDtos
{
    public class CreateCartItemDto
    {
        public string CartId { get; set; }
        public int QuantityId { get; set; }

        //private int _quantity = 1; // Default value of 1
        //public int Quantity { 
        //    get => _quantity; 
        //    set => _quantity = (value == 0) ? 1 : value; }
        public int Quantity { get; set; } = 1;
    }
}
