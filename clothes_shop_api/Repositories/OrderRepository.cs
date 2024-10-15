using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.OrderDtos;
using clothes_shop_api.Helpers;
using clothes_shop_api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace clothes_shop_api.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ecommerceContext _context;
        private readonly IMapper _mapper;

        public OrderRepository(ecommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedList<OrderListDto>> GetAllOrderAsync(PaginationParams paginationParams)
        {
            var query = _context.Orders
                .Include(o => o.Payment)
                .OrderByDescending(x => x.CreateAt)
                .AsQueryable();

            return await PagedList<OrderListDto>.CreateAsync(query.ProjectTo<OrderListDto>(
               _mapper.ConfigurationProvider).AsNoTracking(),
               paginationParams.PageNumber,
               paginationParams.PageSize
               );
        }

        public async Task<OrderDetailDto> GetOrderDetailByIdAsync(int userId, int orderId, string role)
        {
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
    }
}
