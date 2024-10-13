using clothes_shop_api.DTOs.OrderDtos;

namespace clothes_shop_api.DTOs.PaymentDtos
{
    public class PayPalOrderRequestDto
    {
        public OrderRequestDto OrderRequestDto { get; set; }
        public string TransactionId { get; set; }
    }
}
