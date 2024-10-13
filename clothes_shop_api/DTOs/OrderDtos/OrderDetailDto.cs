using clothes_shop_api.Data.Entities;

namespace clothes_shop_api.DTOs.OrderDtos
{
    public class OrderDetailDto
    {
        public int Id { get; set; }
        public string? Note { get; set; }
        public decimal Amount { get; set; }
        public byte Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string Address { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? TransactionId { get; set; }
        public byte Shipping { get; set; }
        public string? Email { get; set; }
        public string PaymentMethod { get; set; }
        public string Provider { get; set; }
        public virtual ICollection<OrderItemDto> OrderItems { get; set; }
    }
}
