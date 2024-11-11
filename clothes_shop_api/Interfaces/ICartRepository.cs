using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs;
using clothes_shop_api.DTOs.CartItemDtos;
//using clothes_shop_api.DTOs.CartDtos;

namespace clothes_shop_api.Interfaces
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        //Task<Cart> GetCartItemByIdAsync(int cartItemId);
        Task<CartDto> GetCartByIdAsync(string cartId);
        //Task<List<CartDto>> GetCartByEmailAsync(string email);
        Task<CartDto> GetUserCartByUserIdAsync(int userId);
        Task<CartItemDto> AddToCartAsync(CreateCartItemDto createCartItemDto, int? userId);
        Task<bool> UpdateCartItemAsync(UpdateCartItemDto updateCartItemDto, int? userId);
        Task<bool> DeleteCartItemAsync(int? userId, string cartId, int cartItemId);
    }
}
