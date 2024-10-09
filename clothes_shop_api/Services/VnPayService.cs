using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.PaymentDtos;
using clothes_shop_api.Helpers;
using clothes_shop_api.Interfaces;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System;

namespace clothes_shop_api.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IOptions<VnPaySettings> _config;

        public VnPayService(IOptions<VnPaySettings> config)
        {
            _config = config;
        }
        public string GetPaymentUrl(HttpContext httpContext, VnPayRequestDto vnPayRequestDto)
        {
            VnPayLibrary vnpay = new VnPayLibrary();
            var tick = DateTime.Now.Ticks;

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _config.Value.TmnCode);
            vnpay.AddRequestData("vnp_Amount", (vnPayRequestDto.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không 
            //mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND
            //(một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần(khử phần thập phân), sau đó gửi sang VNPAY
            //là: 10000000
          
            vnpay.AddRequestData("vnp_CreateDate", vnPayRequestDto.CreateAt.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(httpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", vnPayRequestDto.OrderId.ToString());
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", _config.Value.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", tick.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant.Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY.Không đượctrùng lặp trong ngày
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

            string paymentUrl = vnpay.CreateRequestUrl(_config.Value.Url , _config.Value.HashSecret);

            return paymentUrl;
        }

        public VnPayResponseDto PaymentExecute(IQueryCollection collection)
        {
            VnPayLibrary vnpay = new VnPayLibrary();
            foreach (var (key, value) in collection)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }
            
            long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
            int orderId = Convert.ToInt32(vnpay.GetResponseData("vnp_OrderInfo"));
            long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            string vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            string vnp_SecureHash = collection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            string vnp_TxnRef = vnpay.GetResponseData("vnp_TxnRef");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config.Value.HashSecret);
            if (!checkSignature)
            {
                return new VnPayResponseDto
                {
                    Success = false
                };
            }

            return new VnPayResponseDto
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = orderId,
                TransactionId = vnpayTranId.ToString(),
                //Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode,
                TransactionStatus = vnp_TransactionStatus
            };
        }
    }
}
