using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.OrderDtos;
using clothes_shop_api.DTOs.PaymentDtos;
using clothes_shop_api.Extensions;
using clothes_shop_api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

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

        public OrderController(IUnitOfWork unitOfWork, IMapper mapper, ecommerceContext context, IVnPayService vnPayService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
            _vnPayService = vnPayService;
        }

        [HttpGet("get-user-orders")]
        public async Task<ActionResult<List<Order>>> GetUserOrders()
        {
            var userId = User.GetUserId();
            var userOrders = await _context.Orders
                .Where(x => x.UserId == userId)
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return Ok(userOrders);
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
                
            } else if(orderRequestDto.PaymentMethod == 1) //thanh toan bang VnPay
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
                
            }
            return BadRequest("Invalid payment method");
        }

        [HttpGet("payment-callback")]
        public async Task<ActionResult> PaymentCallback()
        {
            var queryCollection = ControllerContext.HttpContext.Request.Query;
            var vnPayResponse = _vnPayService.PaymentExecute(queryCollection);

            //thanh toan thanh cong
            if(vnPayResponse is not null && vnPayResponse.VnPayResponseCode == "00") 
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
    }
}
