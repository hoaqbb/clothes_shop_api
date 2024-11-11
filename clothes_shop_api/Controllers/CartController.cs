using AutoMapper;
using clothes_shop_api.DTOs;
using clothes_shop_api.DTOs.CartItemDtos;
using clothes_shop_api.Extensions;
using clothes_shop_api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public CartController(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        [HttpGet("get-cart")]
        public async Task<ActionResult<CartDto>> GetCart([FromQuery] string? cartId)
        {
            var userId = User.GetUserId();

            if(userId < 0)
            {
                return Ok(await _unitOfWork.CartRepository.GetCartByIdAsync(cartId));
            }
            //is the user's cart is saved in redis yet
            CartDto userCart;
            if(!string.IsNullOrEmpty(cartId))
            {
                userCart = await _unitOfWork.CartRepository.GetCartByIdAsync(cartId);
                if (userCart != null)
                {
                    return Ok(userCart);
                }
            }
            userCart = await _unitOfWork.CartRepository.GetUserCartByUserIdAsync(userId);
            
            //return Ok(userCart);
            return Ok(await _cacheService.SetDataAsync<CartDto>(userCart.Id, userCart));
        }

        [HttpPost("add-to-cart")]
        public async Task<ActionResult<CartItemDto>> AddToCart(CreateCartItemDto createCartItemDto)
        {
            int? userId = User.GetUserId();
            CartItemDto cartItem;
            if(userId < 0) 
            {
                cartItem = await _unitOfWork.CartRepository.AddToCartAsync(createCartItemDto, null);
                
            } else
            {
                cartItem = await _unitOfWork.CartRepository.AddToCartAsync(createCartItemDto, userId);
            }
            if (cartItem != null) return Ok(cartItem);
            return BadRequest("Them san pham that bai");
        }

        [HttpDelete("remove-cart-item/{cartId}")]
        public async Task<ActionResult> RemoveCartItem(string cartId, int cartItemId)
        {
            var userId = User.GetUserId();
            if (userId < 0)
            {
                return await _unitOfWork.CartRepository.DeleteCartItemAsync(null, cartId, cartItemId) 
                    ? NoContent() 
                    : BadRequest("Co loi khi xoa san pham trong gio hang!");
            }

            if(await _unitOfWork.CartRepository.DeleteCartItemAsync(userId, cartId, cartItemId))
            {
                if (await _unitOfWork.SaveChangesAsync())
                    return NoContent();
            }
            return BadRequest("Co loi khi xoa san pham trong gio hang!");
        }

        [HttpPut("update-cart-item")]
        public async Task<ActionResult<CartDto>> UpdateCartItem(UpdateCartItemDto updateCartItemDto)
        {
            var userId = User.GetUserId();
            if (userId < 0)
            {
                if (await _unitOfWork.CartRepository.UpdateCartItemAsync(updateCartItemDto, null))
                    return NoContent();
            }
            if (await _unitOfWork.CartRepository.UpdateCartItemAsync(updateCartItemDto, userId))
                if(await _unitOfWork.SaveChangesAsync())
                    return NoContent();
            return BadRequest("Co loi khi cap nhat so luong san pham!");
        }

        [HttpPost("set-cache")]
        public async Task<ActionResult> test()
        {
            var id = Guid.NewGuid();
            var a = await _cacheService.SetDataAsync<string>(id.ToString(), "test redis");
            if (a.IsNullOrEmpty()) return BadRequest();
            return Ok(id);
        }

        [HttpGet("get-cache")]
        public async Task<ActionResult<string>> test2(string id)
        {
            var a = await _cacheService.GetDataAsync<CartDto>(id);
            if (a is null) return BadRequest();
            return Ok(a);
        }
        [HttpDelete("delete-cache")]
        public async Task<ActionResult<string>> test3(string id)
        {
            var a = await _cacheService.RemoveDataAsync(id);
            if (a) return Ok();
            return BadRequest(a);
        }
    }
}
