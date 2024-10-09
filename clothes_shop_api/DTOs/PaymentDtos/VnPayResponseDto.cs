namespace clothes_shop_api.DTOs.PaymentDtos
{
    public class VnPayResponseDto
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public int OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
        public string TransactionStatus { get; set; }
    }
}
