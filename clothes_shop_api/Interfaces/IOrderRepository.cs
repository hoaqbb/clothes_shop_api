using clothes_shop_api.DTOs.OrderDtos;
using clothes_shop_api.Helpers;

namespace clothes_shop_api.Interfaces
{
    public interface IOrderRepository
    {
        Task<PagedList<OrderListDto>> GetAllOrderAsync(PaginationParams paginationParams);
        Task<PagedList<OrderListDto>> GetUserOrdersAsync(PaginationParams paginationParams, int userId);
        Task<OrderDetailDto> GetOrderDetailByIdAsync(int userId, int orderId, string role);
        Task<bool> UpdateOrderAsync(int id);
    }
}
