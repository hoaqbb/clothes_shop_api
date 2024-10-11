﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.OrderDtos;
using clothes_shop_api.DTOs.PaymentDtos;
using clothes_shop_api.Extensions;
using clothes_shop_api.Helpers;
using clothes_shop_api.Interfaces;
using clothes_shop_api.Services;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using static clothes_shop_api.Services.PayPalClient;
using System.Text;
using System.Text.Json;

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ecommerceContext _context;
        private readonly IVnPayService _vnPayService;
        private readonly PayPalClient _payPalClient;

        public OrderController(IUnitOfWork unitOfWork, IMapper mapper, ecommerceContext context, IVnPayService vnPayService, IConfiguration config, PayPalClient payPalClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
            _vnPayService = vnPayService;
            _payPalClient = payPalClient;
        }

        [Authorize]
        [HttpGet("get-user-orders")]
        public async Task<ActionResult<List<Order>>> GetUserOrders([FromQuery]PaginationParams paginationParams)
        {
            var userId = User.GetUserId();

            var userOrders = await _unitOfWork.OrderRepository.GetUserOrdersAsync(paginationParams, userId);

            Response.AddPaginationHeader(userOrders.CurrentPage, userOrders.PageSize,
                userOrders.TotalCount, userOrders.TotalPages);

            return Ok(userOrders);
        }

        [Authorize]
        [HttpGet("get-order-detail/{orderId}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderDetail(int orderId)
        {
            var userId = User.GetUserId();
            var order = await _unitOfWork.OrderRepository.GetOrderDetailByIdAsync(userId, orderId);
            if(order is not null)
            {
                return Ok(order);
            }

            return NotFound();
        }

        [HttpPost("create-order")]
        public async Task<ActionResult> CreateOrder(OrderRequestDto orderRequestDto)
        {
            var userId = User.GetUserId();
            //thanh toan COD
            if (orderRequestDto.PaymentMethod == 0)
            {
                var order = new Order
                {
                    Id = new Random().Next(1000, 10000),
                    Fullname = orderRequestDto.Fullname,
                    Email = orderRequestDto.Email,
                    Address = orderRequestDto.Address,
                    Amount = orderRequestDto.Amount,
                    Note = orderRequestDto.Note,
                    PhoneNumber = orderRequestDto.PhoneNumber,
                    Shipping = orderRequestDto.Shipping,
                    Status = 0,
                    UserId = userId,
                };

                await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Orders.Add(order);
                    //await _context.SaveChangesAsync();
                    var userCart = await _context.Carts
                        .Where(x => x.UserId == userId)
                        .ToListAsync();

                    var payment = new Payment
                    {
                        OrderId = order.Id,
                        Amount = orderRequestDto.Amount,
                        Method = "COD",
                        UserId = userId,
                    };
                    _context.Add(payment);
                    await _context.SaveChangesAsync();
                    order.PaymentId = payment.Id;

                    var orderItems = new List<OrderItem>();
                    foreach (var item in userCart)
                    {
                        _context.Add(new OrderItem
                        {
                            CartId = item.Id,
                            OrderId = order.Id,
                            QuantityId = item.QuantityId,
                            Quantity = item.Quantity
                        });
                    }
                    
                    _context.AddRange(orderItems);
                    _context.RemoveRange(userCart);
                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();

                    return Ok("Success");
                }
                catch (Exception ex)
                {
                   await _context.Database.RollbackTransactionAsync();
                   var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                   return BadRequest($"Transaction failed: {innerExceptionMessage}");
                }
                
            } 
            else if(orderRequestDto.PaymentMethod == 1) //thanh toan bang VnPay
            {
                try
                {
                    var vnPayRequest = new VnPayRequestDto
                    {
                        Amount = orderRequestDto.Amount,
                        UserId = userId,
                        Description = orderRequestDto.Note,
                        Fullname = orderRequestDto.Fullname
                    };
                    await _context.Database.BeginTransactionAsync();
                    var payment = new Payment
                    {
                        Amount = orderRequestDto.Amount,
                        Method = "Internet Banking",
                        Provider = "VnPay",
                        Status = false,
                        OrderId = vnPayRequest.OrderId,
                        UserId = userId
                    };
                    _context.Add(payment);
                    await _context.SaveChangesAsync();

                    var order = new Order
                    {
                        Id = vnPayRequest.OrderId,
                        Amount = orderRequestDto.Amount,
                        Address = orderRequestDto.Address,
                        Email = orderRequestDto.Email,
                        Fullname= orderRequestDto.Fullname,
                        Note = orderRequestDto.Note,
                        PaymentId = payment.Id,
                        Status = 0,
                        UserId= userId,
                        PhoneNumber = orderRequestDto.PhoneNumber,
                    };
                    _context.Add(order);
                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();
                    
                    var returnUrl = _vnPayService.GetPaymentUrl(HttpContext, vnPayRequest);

                    return Ok(returnUrl);
                } catch (Exception ex)
                {
                    await _context.Database.RollbackTransactionAsync();
                    var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                    return BadRequest($"Transaction failed: {innerExceptionMessage}");
                }
                
            } else if (orderRequestDto.PaymentMethod == 2) //thanh toan bang PayPal
            {
                var paypalResponse = await _payPalClient.CreatePayPalOrderAsync(orderRequestDto.Amount/23000);
                if(paypalResponse is not null)
                {
                    try
                    {
                        var orderId = new Random().Next(1000, 10000);
                        await _context.Database.BeginTransactionAsync();
                        var payment = new Payment
                        {
                            Amount = orderRequestDto.Amount,
                            Method = "Internet Banking",
                            Provider = "PayPal",
                            Status = false,
                            TransactionId = paypalResponse.id,
                            OrderId = orderId,
                            UserId = userId
                        };
                        _context.Add(payment);
                        await _context.SaveChangesAsync();

                        var order = new Order
                        {
                            Id = orderId,
                            Amount = orderRequestDto.Amount,
                            Address = orderRequestDto.Address,
                            Email = orderRequestDto.Email,
                            Fullname = orderRequestDto.Fullname,
                            Note = orderRequestDto.Note,
                            PaymentId = payment.Id,
                            Status = 0,
                            UserId = userId,
                            PhoneNumber = orderRequestDto.PhoneNumber,
                        };
                        _context.Add(order);
                        await _context.SaveChangesAsync();
                        await _context.Database.CommitTransactionAsync();

                        return Ok(paypalResponse);
                    }
                    catch (Exception ex)
                    {
                        await _context.Database.RollbackTransactionAsync();
                        var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                        return BadRequest($"Transaction failed: {innerExceptionMessage}");
                    }
                }
                return BadRequest();
                
            }
            return BadRequest("Invalid payment method");
        }

        [HttpGet("payment-callback")]
        public async Task<ActionResult> VnPayPaymentCallback()
        {
            var queryCollection = ControllerContext.HttpContext.Request.Query;
            var vnPayResponse = _vnPayService.PaymentExecute(queryCollection);

            //thanh toan thanh cong
            if(vnPayResponse is not null && vnPayResponse.VnPayResponseCode == "00" 
                && vnPayResponse.Success == true) 
            {
                var userId = User.GetUserId();
                var order = await _context.Orders.SingleOrDefaultAsync(x => x.Id == vnPayResponse.OrderId);
                if (order is not null)
                {
                    try
                    {
                        await _context.Database.BeginTransactionAsync();
                        order.Status = 1;
                        var payment = await _context.Payments.SingleOrDefaultAsync(x => x.OrderId == order.Id);
                        payment.Status = true;
                        payment.TransactionId = vnPayResponse.TransactionId;
                        _context.Payments.Update(payment);
                        _context.Orders.Update(order);

                        var userCart = await _context.Carts
                            .Where(x => x.UserId == userId)
                            .ToListAsync();

                        var orderItems = new List<OrderItem>();
                        foreach (var item in userCart)
                        {
                            _context.Add(new OrderItem
                            {
                                CartId = item.Id,
                                OrderId = order.Id,
                                QuantityId = item.QuantityId,
                                Quantity = item.Quantity
                            });
                        }

                        _context.AddRange(orderItems);
                        _context.RemoveRange(userCart);
                        await _context.SaveChangesAsync();
                        await _context.Database.CommitTransactionAsync();

                        return Ok(vnPayResponse);

                    } catch (Exception ex)
                    {
                        await _context.Database.RollbackTransactionAsync();
                        var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                        return BadRequest($"Transaction failed: {innerExceptionMessage}");
                    }

                }
                return BadRequest();
            } else
            {
                try
                {
                    var order = await _context.Orders.SingleOrDefaultAsync(x => x.Id == vnPayResponse.OrderId);
                    if (order is not null)
                    {
                        var payment = await _context.Payments.SingleOrDefaultAsync(x => x.OrderId == order.Id);
                        _context.Payments.Remove(payment);
                        _context.Orders.Remove(order);
                        await _context.SaveChangesAsync();
                    }
                } catch (Exception ex)
                {
                    var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                    return BadRequest($"Transaction failed: {innerExceptionMessage}");
                }
                //thanh toan khong thanh cong
                return BadRequest("thanh toan khong thanh cong");
            }

        }

        //[HttpPost("paypal-order")]
        //public async Task<ActionResult<Order>> PayPalOrder(decimal amount)
        //{
        //    try
        //    {
        //        var orderId = await _payPalClient.CreatePayPalOrderAsync(amount);
        //        return Ok(orderId);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
            
        //}

        //[HttpGet("capture-paypal-order")]
        //public async Task<ActionResult> CapturePayPalOrder(string id)
        //{
        //    var orderResponse = await _payPalClient.CaptureOrderAsync(id);
        //    if(orderResponse is not null)
        //    {
        //        try
        //        {


        //            return Ok(orderResponse);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }
            
        //}

        //[HttpGet("token")]
        //public async Task<string> GetAccessTokenAsync()
        //{
        //    var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{c}:{a}"));

        //    //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", (authToken));
        //    var content = new List<KeyValuePair<string, string>>
        //    {
        //        new("grant_type", "client_credentials")
        //    };
        //    var request = new HttpRequestMessage
        //    {
        //        RequestUri = new Uri($"{BaseUrl}/v1/oauth2/token"),
        //        Method = System.Net.Http.HttpMethod.Post,
        //        Headers =
        //        {
        //            { "Authorization", $"Basic {authToken}" }
        //        },
        //        Content = new FormUrlEncodedContent(content)
        //    };


        //    var httpClient = new HttpClient();
        //    var httpResponse = await httpClient.SendAsync(request);
        //    var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
        //    var response = JsonSerializer.Deserialize<AuthResponse>(jsonResponse);

        //    //var jsonResponse = await response.Content.ReadAsStringAsync();
        //    //var tokenResponse = JsonSerializer.Deserialize<AuthResponse>(jsonResponse);

        //    return response?.access_token;
        //}
      
    }
}
