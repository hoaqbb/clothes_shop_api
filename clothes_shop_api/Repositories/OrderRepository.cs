using clothes_shop_api.Data.Entities;
using clothes_shop_api.Interfaces;

namespace clothes_shop_api.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ecommerceContext _context;

        public OrderRepository(ecommerceContext context)
        {
            _context = context;
        }
    }
}
