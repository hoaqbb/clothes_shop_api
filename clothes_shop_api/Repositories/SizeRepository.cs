using clothes_shop_api.Data.Entities;
using clothes_shop_api.Interfaces;

namespace clothes_shop_api.Repositories
{
    public class SizeRepository : GenericRepository<Size>, ISizeRepository
    {
        public SizeRepository(ecommerce_decryptedContext context) : base(context)
        {
        }
    }
}
