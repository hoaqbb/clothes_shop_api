using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.CartDtos;

namespace clothes_shop_api.Interfaces
{
    public interface ICartRepository
    {
        void DeleteCartItem(Cart cart);
        Task<Cart> GetCartItemByIdAsync(int cartItemId);
        Task<List<CartDto>> GetCartByEmailAsync(string email);
        Task<Cart> AddToCartAsync(CreateCartItemDto createCartItemDto, int userId);
    }
}
