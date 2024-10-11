using clothes_shop_api.Data.Entities;

namespace clothes_shop_api.DTOs.OrderDtos
{
    public class OrderListDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public byte Status { get; set; }
        public DateTime CreateAt { get; set; }
        public string Address { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string PaymentMethod { get; set; }
    }
}
