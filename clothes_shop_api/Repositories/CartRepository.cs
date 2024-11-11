using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs;
using clothes_shop_api.DTOs.CartItemDtos;
using clothes_shop_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clothes_shop_api.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly ecommerce_decryptedContext _context;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public CartRepository(ecommerce_decryptedContext context, IMapper mapper, ICacheService cacheService) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public void DeleteCartItem(CartItem cartItem)
        {   
            _context.CartItems.Remove(cartItem);
        }

        public async Task<CartDto> GetCartByIdAsync(string cartId)
        {
            return await _cacheService.GetDataAsync<CartDto>(cartId);
        }

        public async Task<CartDto> GetUserCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .ProjectTo<CartDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<CartItemDto> AddToCartAsync(CreateCartItemDto createCartItemDto, int? userId)
        {
            var newCartItem = await _context.Quantities
                        .ProjectTo<CartItemDto>(_mapper.ConfigurationProvider)
                        .FirstOrDefaultAsync(x => x.Id == createCartItemDto.QuantityId);

            if(newCartItem is null || newCartItem.ProductVariant.Amount == 0) return null;

            var cart = await _cacheService.GetDataAsync<CartDto>(createCartItemDto.CartId);

            //create a cart save to redis if user is not login
            if (userId is null)
            {
                if(cart is null)
                {
                    newCartItem.Quantity = 1;
                    cart = new CartDto
                    {
                        Id = createCartItemDto.CartId,
                        CartItems = new List<CartItemDto> { newCartItem }
                    };
                    await _cacheService.SetDataAsync<CartDto>(createCartItemDto.CartId, cart);
                    return newCartItem;
                }

                var cartItemIndex = cart.CartItems.FindIndex(x => x.ProductVariant.Id == createCartItemDto.QuantityId);
                if (cartItemIndex >= 0)
                {
                    cart.CartItems[cartItemIndex].Quantity += 1;

                    if (cart.CartItems[cartItemIndex].Quantity > newCartItem.ProductVariant.Amount)
                    {
                        cart.CartItems[cartItemIndex].Quantity = newCartItem.ProductVariant.Amount;
                    }
                    await _cacheService.SetDataAsync<CartDto>(createCartItemDto.CartId, cart);

                    return cart.CartItems[cartItemIndex];
                }

                newCartItem.Quantity = 1;
                cart.CartItems.Add(newCartItem);
                await _cacheService.SetDataAsync<CartDto>(createCartItemDto.CartId, cart);

                return newCartItem;
            } 
            //update cart of user to database and get to redis
            else
            {
                //var userCart = await GetUserCartByUserIdAsync((int)userId);
                //if (userCart is null) return null;

                //var cartItemIndex = userCart.CartItems.FindIndex(x => x.ProductVariant.Id == createCartItemDto.QuantityId);

                if (cart is null)
                {
                    var userCart = await GetUserCartByUserIdAsync((int)userId);
                    if (userCart is null) return null;

                    var cartItemIndex = userCart.CartItems.FindIndex(x => x.ProductVariant.Id == createCartItemDto.QuantityId);
                    //var n = await CalculateCartItemQuantity(userCart, newCartItem, createCartItemDto.QuantityId);
                    //if(n is not null) 
                    //{

                    //}

                    //var cartItemIndex = userCart.CartItems.FindIndex(x => x.ProductVariant.Id == createCartItemDto.QuantityId);
                    if (cartItemIndex >= 0)
                    {
                        userCart.CartItems[cartItemIndex].Quantity += 1;

                        if (userCart.CartItems[cartItemIndex].Quantity > newCartItem.ProductVariant.Amount)
                        {
                            userCart.CartItems[cartItemIndex].Quantity = newCartItem.ProductVariant.Amount;
                            await _cacheService.SetDataAsync<CartDto>(createCartItemDto.CartId, userCart);
                            return userCart.CartItems[cartItemIndex];
                        }
                        await _cacheService.SetDataAsync<CartDto>(createCartItemDto.CartId, userCart);

                        _context.Update(userCart.CartItems[cartItemIndex]);

                        return userCart.CartItems[cartItemIndex];
                    }
                    newCartItem.Quantity = 1;
                    userCart.CartItems.Add(newCartItem);
                    var item = new CartItem
                    {
                        CartId = createCartItemDto.CartId,
                        QuantityId = newCartItem.ProductVariant.Id,
                        Quantity = newCartItem.Quantity
                    };
                    _context.Add(item);
                    await _cacheService.SetDataAsync<CartDto>(createCartItemDto.CartId, cart);

                    return newCartItem;
                }
                var cartItemIndex2 = cart.CartItems.FindIndex(x => x.ProductVariant.Id == createCartItemDto.QuantityId);
                //user's cart is stored in redis
                if (cartItemIndex2 >= 0)
                {
                    cart.CartItems[cartItemIndex2].Quantity += 1;

                    if (cart.CartItems[cartItemIndex2].Quantity > newCartItem.ProductVariant.Amount)
                    {
                        cart.CartItems[cartItemIndex2].Quantity = newCartItem.ProductVariant.Amount;
                        await _cacheService.SetDataAsync<CartDto>(createCartItemDto.CartId, cart);
                        return cart.CartItems[cartItemIndex2];
                    }
                    await _cacheService.SetDataAsync<CartDto>(createCartItemDto.CartId, cart);
                    var a = await _context.FindAsync<CartItem>(cart.CartItems[cartItemIndex2].Id);
                    if (a != null) a.Quantity = cart.CartItems[cartItemIndex2].Quantity;
                    _context.Update<CartItem>(a);
                    await _context.SaveChangesAsync();
                    return cart.CartItems[cartItemIndex2];
                }
                newCartItem.Quantity = 1;
                
                var item1 = new CartItem
                {
                    CartId = createCartItemDto.CartId,
                    QuantityId = newCartItem.ProductVariant.Id,
                    Quantity = newCartItem.Quantity
                };
                _context.Add(item1);
                if(await _context.SaveChangesAsync() > 0)
                {
                    newCartItem.Id = item1.Id;
                    cart.CartItems.Add(newCartItem);
                    await _cacheService.SetDataAsync<CartDto>(createCartItemDto.CartId, cart);

                    return newCartItem;
                }

                throw new Exception();
            }
        }

        //private bool IsAvailable(int quantityId)
        //{

        //}

        public async Task<bool> UpdateCartItemAsync(UpdateCartItemDto updateCartItemDto, int? userId)
        {
            var cart = await _cacheService.GetDataAsync<CartDto>(updateCartItemDto.CartId);
            if (cart == null) return false;

            var item = cart.CartItems.FirstOrDefault(x => x.Id == updateCartItemDto.CartItemId);
            item.Quantity = updateCartItemDto.Quantity;
            if(userId != null)
            {
                var cartItem = await _context.CartItems.FindAsync(updateCartItemDto.CartItemId);
                if (cartItem is null) return false;
                cartItem.Quantity = updateCartItemDto.Quantity;
            }

            await _cacheService.SetDataAsync<CartDto>(cart.Id, cart);
            
            return true;
        }

        private async Task<CartItemDto> CalculateCartItemQuantity(CartDto cart, CartItemDto newCartItem, int quantityId)
        {
            var cartItemIndex = cart.CartItems.FindIndex(x => x.ProductVariant.Id == quantityId);
            if (cartItemIndex >= 0)
            {
                cart.CartItems[cartItemIndex].Quantity += 1;

                if (cart.CartItems[cartItemIndex].Quantity > newCartItem.ProductVariant.Amount)
                {
                    cart.CartItems[cartItemIndex].Quantity = newCartItem.ProductVariant.Amount;
                }
                //await _cacheService.SetDataAsync<CartDto>(cart.Id, cart);

                return cart.CartItems[cartItemIndex];
            }

            return null;
        }

        public async Task<bool> DeleteCartItemAsync(int? userId, string cartId, int cartItemId)
        {
            var cart = await GetCartByIdAsync(cartId);
            var item = cart.CartItems.SingleOrDefault(x => x.Id == cartItemId);
            if(cart.CartItems.Remove(item))
            {
                if (userId != null)
                {
                    var cartItem = await _context.CartItems.SingleOrDefaultAsync(x => x.Id == cartItemId);
                    _context.Remove(cartItem);
                }
                await _cacheService.SetDataAsync<CartDto>(cartId, cart);
                return true;
            }
            return false;
        }
    }
}
