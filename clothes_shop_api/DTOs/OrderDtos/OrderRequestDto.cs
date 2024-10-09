namespace clothes_shop_api.DTOs.OrderDtos
{
    public class OrderRequestDto
    {
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public byte Shipping { get; set; }
        public string Note { get; set; }
        public int Amount { get; set; }
        public string Address { get; set; } = null!;
        public int PaymentMethod { get; set; }
    }
}
