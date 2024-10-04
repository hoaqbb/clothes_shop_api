using AutoMapper;
using clothes_shop_api.DTOs.CartDtos;
using clothes_shop_api.Extensions;
using clothes_shop_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("get-user-cart")]
        public async Task<ActionResult<List<CartDto>>> GetUserCart()
        {
            var email = User.GetEmail();
            return Ok(await _unitOfWork.CartRepository.GetCartByEmailAsync(email));
        }

        [HttpPost("add-to-cart")]
        public async Task<ActionResult<CartDto>> AddToCart(CreateCartItemDto createCartItemDto)
        {
            var userId = User.GetUserId();
            var cartItem = await _unitOfWork.CartRepository.AddToCartAsync(createCartItemDto, userId);

            if(cartItem is not null)
            {
                await _unitOfWork.SaveAllAsync();
                cartItem = await _unitOfWork.CartRepository.GetCartItemByIdAsync(cartItem.Id);
                return Ok(_mapper.Map<CartDto>(cartItem));
            }
            return BadRequest("Them san pham that bai");
        }

        [HttpDelete("remove-cart-item/{id}")]
        public async Task<ActionResult> RemoveCartItem(int id)
        {
            var userId = User.GetUserId();
            var cartItem = await _unitOfWork.CartRepository.GetCartItemByIdAsync(id);

            if(cartItem is not null)
            {
                if (cartItem.UserId != userId) return Unauthorized();

                _unitOfWork.CartRepository.DeleteCartItem(cartItem);

                if (await _unitOfWork.SaveAllAsync()) return Ok();
            }

            return BadRequest("Co loi khi xoa item");
        }
    }
}
