using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.CartDtos;
using clothes_shop_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clothes_shop_api.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ecommerceContext _context;
        private readonly IMapper _mapper;

        public CartRepository(ecommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void DeleteCartItem(Cart cart)
        {
            _context.Carts.Remove(cart);
        }

        public async Task<Cart> GetCartItemByIdAsync(int cartItemId)
        {
            return await _context.Carts
                .Include(c => c.QuantityNavigation).ThenInclude(c => c.Product)
                .Include(c => c.QuantityNavigation.ProductColor.Color)
                .Include(c => c.QuantityNavigation.Product.ProductImages)
                .Include(c => c.QuantityNavigation.Size)
                .Where(c => c.Id == cartItemId)
                .SingleOrDefaultAsync();
        }

        public async Task<List<CartDto>> GetCartByEmailAsync(string email)
        {
            var cart = await _context.Carts
                .Where(x => x.User.Email == email)
                .ProjectTo<CartDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return cart;
        }

        public async Task<Cart> AddToCartAsync(CreateCartItemDto createCartItemDto, int userId)
        {
            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.QuantityId == createCartItemDto.QuantityId);
            var quantityItem = await _context.Quantities
                .SingleOrDefaultAsync(q => q.Id == createCartItemDto.QuantityId);

            if (cartItem != null && quantityItem != null)
            {
                if(createCartItemDto.Quantity > 1)
                {
                    cartItem.Quantity = createCartItemDto.Quantity;
                }
                else
                {
                    cartItem.Quantity += createCartItemDto.Quantity;
                }
                
                if (cartItem.Quantity > quantityItem.Amount)
                {
                    cartItem.Quantity = quantityItem.Amount;
                }

                return cartItem;
            }
            cartItem = new Cart
            {
                UserId = userId,
                Quantity = createCartItemDto.Quantity,
                QuantityId = createCartItemDto.QuantityId
            };

            await _context.Carts.AddAsync(cartItem);

            return cartItem;
        }

    }
}
