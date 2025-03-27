using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.OrderDtos;
using clothes_shop_api.Helpers;
using clothes_shop_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clothes_shop_api.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ecommerce_decryptedContext _context;
        private readonly IMapper _mapper;

        public OrderRepository(ecommerce_decryptedContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedList<OrderListDto>> GetAllOrderAsync(AdminOrderParams adminOrderParams)
        {
            IQueryable<OrderListDto> query = _context.Orders
                .ProjectTo<OrderListDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

            if (await query.AnyAsync())
            {
                query = adminOrderParams.SortBy switch
                {
                    "created_ascending" => query.OrderBy(x => x.CreateAt),
                    "price_ascending" => query.OrderBy(x => x.Amount),
                    "price_descending" => query.OrderByDescending(x => x.Amount),
                    _ => query.OrderByDescending(x => x.CreateAt)
                };

                query = adminOrderParams.Status switch
                {
                    "pending" => query.Where(x => x.Status == 0),
                    "confirmed" => query.Where(x => x.Status == 1),
                    "on_delivery" => query.Where(x => x.Status == 2),
                    "completed" => query.Where(x => x.Status == 3),
                    "cancelled" => query.Where(x => x.Status == 100),
                    _ => query
                };

                query = adminOrderParams.PaymentMethod switch
                {
                    "cod" => query.Where(x => x.PaymentMethod == "COD"),
                    "internet_banking" => query.Where(x => x.PaymentMethod == "Internet Banking"),
                    _ => query
                };
            }

            return await PagedList<OrderListDto>.CreateAsync(
                query.AsNoTracking(),
                adminOrderParams.PageNumber,
                adminOrderParams.PageSize
                );
        }

        public async Task<OrderDetailDto> GetOrderDetailByIdAsync(int userId, int orderId, string role)
        {
            if (userId < 0) return null;

            if(role == "Customer")
            {
                var order = await _context.Orders
                .Where(x => x.Id == orderId && x.UserId == userId)
                .ProjectTo<OrderDetailDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
                return order;
            }
            else
            {
                var order = await _context.Orders
                    .Where(x => x.Id == orderId)
                    .ProjectTo<OrderDetailDto>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync();

                return order;
            }
            
        }

        public async Task<PagedList<OrderListDto>> GetUserOrdersAsync(PaginationParams paginationParams, int userId)
        {
            var query = _context.Orders
                .Include(o => o.Payment)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreateAt)
                .AsQueryable();

            return await PagedList<OrderListDto>.CreateAsync(query.ProjectTo<OrderListDto>(
               _mapper.ConfigurationProvider).AsNoTracking(), 
               paginationParams.PageNumber, 
               paginationParams.PageSize
               );
        }

        public async Task<bool> UpdateOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order is null)
            {
                return false;
            }
            order.Status += 1;
            order.UpdateAt = DateTime.Now;
            _context.Orders.Update(order);
            if(await _context.SaveChangesAsync() > 0)
                return true;

            return false;
        }

        public async Task CancleOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if(order is null || order.Status != 0) return;

            order.Status = 100;
            order.UpdateAt = DateTime.Now;

            return;
        }
    }
}
