namespace clothes_shop_api.DTOs.PaymentDtos
{
    public class VnPayRequestDto
    {
        public string Fullname { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public DateTime CreateAt { get; set; }
        public int OrderId { get; set; } = new Random().Next(1000, 10000);
        public int UserId { get; set; }
    }
}
