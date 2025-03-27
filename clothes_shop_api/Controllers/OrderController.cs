using AutoMapper;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs;
using clothes_shop_api.DTOs.CartItemDtos;
using clothes_shop_api.DTOs.OrderDtos;
using clothes_shop_api.DTOs.PaymentDtos;
using clothes_shop_api.Extensions;
using clothes_shop_api.Helpers;
using clothes_shop_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ecommerce_decryptedContext _context;
        private readonly IVnPayService _vnPayService;
        private readonly ICacheService _cacheService;

        public OrderController(IUnitOfWork unitOfWork, IMapper mapper, 
            ecommerce_decryptedContext context, IVnPayService vnPayService, 
            ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
            _vnPayService = vnPayService;
            _cacheService = cacheService;
        }

        [Authorize]
        [HttpGet("get-user-orders")]
        public async Task<ActionResult<List<OrderListDto>>> GetUserOrders([FromQuery] PaginationParams paginationParams)
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
            if (userId < 0) return Unauthorized();

            string role = User.GetRole();
            var order = await _unitOfWork.OrderRepository.GetOrderDetailByIdAsync(userId, orderId, role);
            if (order is not null)
            {
                return Ok(order);
            }

            return NotFound();
        }

        [HttpPost("create-order")]
        public async Task<ActionResult> CreateOrder(OrderRequestDto orderRequestDto)
        {
            int? userId = User.GetUserId();
            var cart = await _cacheService.GetDataAsync<CartDto>(orderRequestDto.CartId);
            if (cart == null) return BadRequest("Khong tim thay gio hang"); 
            if (cart.CartItems.Count < 1) return BadRequest("Gio hang trong");

            //check is user login
            if (userId < 0) userId = null;

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
                    ShippingFee = orderRequestDto.ShippingFee,
                    Note = orderRequestDto.Note,
                    PhoneNumber = orderRequestDto.PhoneNumber,
                    DeliveryMethod = orderRequestDto.DeliveryMethod,
                    Status = 0,
                    UserId = userId,
                };

                await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Orders.Add(order);

                    var payment = new Payment
                    {
                        OrderId = order.Id,
                        Amount = orderRequestDto.Amount,
                        Method = "COD",
                        UserId = userId
                    };

                    _context.Add(payment);
                    await _context.SaveChangesAsync();
                    order.PaymentId = payment.Id;

                    foreach (var item in cart.CartItems)
                    {
                        _context.Add(new OrderItem
                        {
                            OrderId = order.Id,
                            QuantityId = item.ProductVariant.Id,
                            Quantity = item.Quantity
                        });
                    }

                    if (userId != null)
                    {
                        var userCart = await _context.Carts
                            .Include(x => x.CartItems)
                            .SingleOrDefaultAsync(x => x.UserId == userId);

                        foreach (var item in cart.CartItems)
                        {
                            _context.Add(new OrderItem
                            {
                                OrderId = order.Id,
                                QuantityId = item.ProductVariant.Id,
                                Quantity = item.Quantity
                            });
                        }
                        _context.RemoveRange(userCart.CartItems);
     
                    }               
                    
                    await _context.SaveChangesAsync();
                    cart.CartItems.Clear();
                    await _cacheService.SetDataAsync(cart.Id, cart);
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
            else if (orderRequestDto.PaymentMethod == 1) //thanh toan bang VnPay
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
                    
                    var payment = new Payment
                    {
                        Amount = orderRequestDto.Amount,
                        Method = "Internet Banking",
                        Provider = "VnPay",
                        Status = false,
                        OrderId = vnPayRequest.OrderId,
                        UserId = userId
                    };

                    await _unitOfWork.BeginTransactionAsync();
                    _context.Add(payment);
                    await _context.SaveChangesAsync();

                    var order = new Order
                    {
                        Id = vnPayRequest.OrderId,
                        Amount = orderRequestDto.Amount,
                        Address = orderRequestDto.Address,
                        Email = orderRequestDto.Email,
                        Fullname = orderRequestDto.Fullname,
                        Note = orderRequestDto.Note,
                        ShippingFee = orderRequestDto.ShippingFee,
                        PaymentId = payment.Id,
                        Status = 100, //bi huy
                        UserId = userId,
                        PhoneNumber = orderRequestDto.PhoneNumber,
                    };
                    _context.Add(order);
                    await _context.SaveChangesAsync();

                    foreach (var item in cart.CartItems)
                    {
                        _context.Add(new OrderItem
                        {
                            OrderId = order.Id,
                            QuantityId = item.ProductVariant.Id,
                            Quantity = item.Quantity
                        });
                    }

                    await _context.SaveChangesAsync();
                    cart.CartItems.Clear();
                    await _cacheService.SetDataAsync(cart.Id, cart);
                    await _unitOfWork.CommitAsync();

                    var returnUrl = _vnPayService.GetPaymentUrl(HttpContext, vnPayRequest);

                    return Ok(returnUrl);
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                    var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                    return BadRequest($"Transaction failed: {innerExceptionMessage}");
                }

            }

            return BadRequest("Invalid payment method");
        }

        [HttpGet("payment-callback")]
        public async Task<ActionResult> VnPayPaymentCallback()
        {
            var queryCollection = ControllerContext.HttpContext.Request.Query;
            var vnPayResponse = _vnPayService.PaymentExecute(queryCollection);

            var order = await _context.Orders
                .Include(x => x.Payment)
                .SingleOrDefaultAsync(x => x.Id == vnPayResponse.OrderId);

            //thanh toan thanh cong
            if (vnPayResponse != null && vnPayResponse.VnPayResponseCode == "00"
                && vnPayResponse.Success == true)
            {
                int? userId = User.GetUserId();
                if (userId < 1) userId = null;

                if (order != null)
                {
                    try
                    {
                        await _unitOfWork.BeginTransactionAsync();
                        order.Status = 1;
                        order.Payment.Status = true;
                        order.Payment.TransactionId = vnPayResponse.TransactionId;
                        _context.Payments.Update(order.Payment);
                        _context.Orders.Update(order);

                        if (userId != null)
                        {
                            var userCart = await _context.Carts
                                .Include(x => x.CartItems)
                                .SingleOrDefaultAsync(x => x.UserId == userId);

                            _context.RemoveRange(userCart.CartItems);
                            await _cacheService.SetDataAsync(userCart.Id, new CartDto
                            {
                                Id = userCart.Id,
                                UserId = userCart.UserId,
                                CartItems = new List<CartItemDto> { }
                            });
                        }

                        await _context.SaveChangesAsync();

                        await _unitOfWork.CommitAsync();

                        return Ok(vnPayResponse);

                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackAsync();
                        var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                        return BadRequest($"Transaction failed: {innerExceptionMessage}");
                    }

                }
                return BadRequest("Loi");
            }
            else
            {
                try
                {
                    
                    if (order != null)
                    {
                        var payment = await _context.Payments.SingleOrDefaultAsync(x => x.OrderId == order.Id);
                        _context.Payments.Remove(payment);
                        _context.Orders.Remove(order);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                    return BadRequest($"Transaction failed: {innerExceptionMessage}");
                }
                //thanh toan khong thanh cong
                return BadRequest("thanh toan khong thanh cong");
            }

        }

        [HttpPost("create-paypal-order")]
        public async Task<ActionResult> CreatePayPalOrder(PayPalOrderRequestDto payPalOrderRequestDto)
        {
            int? userId = User.GetUserId();
            var orderRequestDto = payPalOrderRequestDto.OrderRequestDto;
            var transactionId = payPalOrderRequestDto.TransactionId;
            var cart = await _cacheService.GetDataAsync<CartDto>(orderRequestDto.CartId);
            if (cart == null) return BadRequest("Khong tim thay gio hang");
            if (cart.CartItems.Count < 1) return BadRequest("Gio hang trong");

            //check is user login
            if (userId < 0) userId = null;
            try
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
                    DeliveryMethod = orderRequestDto.DeliveryMethod,
                    ShippingFee = orderRequestDto.ShippingFee,
                    Status = 1,
                    UserId = userId,
                };

                await _unitOfWork.BeginTransactionAsync();
                await _context.Orders.AddAsync(order);

                

                var payment = new Payment
                {
                    OrderId = order.Id,
                    Amount = orderRequestDto.Amount,
                    Method = "Internet Banking",
                    TransactionId = transactionId,
                    Provider = "PayPal",
                    Status = true,
                    UserId = userId
                };
                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();
                order.PaymentId = payment.Id;

                foreach (var item in cart.CartItems)
                {
                    _context.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        QuantityId = item.ProductVariant.Id,
                        Quantity = item.Quantity
                    });
                }

                if (userId != null)
                {
                    var userCart = await _context.Carts
                        .Include(c => c.CartItems)
                        .SingleOrDefaultAsync(x => x.UserId == userId);

                    _context.RemoveRange(userCart.CartItems);
                }

                await _context.SaveChangesAsync();

                await _unitOfWork.CommitAsync();

                cart.CartItems.Clear();
                await _cacheService.SetDataAsync(cart.Id, cart);
                return Ok();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-status-order/{id}")]
        public async Task<ActionResult> UpdateOrder(int id)
        {
            var isUpdated = await _unitOfWork.OrderRepository.UpdateOrderAsync(id);
            if(!isUpdated)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("cancle-order/{id}")]
        public async Task<ActionResult> CancleOrder(int id)
        {
            await _unitOfWork.OrderRepository.CancleOrderAsync(id);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return NoContent();
            }
            return BadRequest();
        }

    }
}
