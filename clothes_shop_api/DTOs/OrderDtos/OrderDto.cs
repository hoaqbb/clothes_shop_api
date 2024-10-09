using clothes_shop_api.Data.Entities;

namespace clothes_shop_api.DTOs.OrderDtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Note { get; set; }
        public decimal Amount { get; set; }
        public byte Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string Address { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public int PhoneNumber { get; set; }
        public byte Shipping { get; set; }
        public string PaymentMethod { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
