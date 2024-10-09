using clothes_shop_api.DTOs.PaymentDtos;

namespace clothes_shop_api.Interfaces
{
    public interface IVnPayService
    {
        string GetPaymentUrl(HttpContext httpContext, VnPayRequestDto vnPayRequestDto);
        VnPayResponseDto PaymentExecute(IQueryCollection collection);
    }
}
