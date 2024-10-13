using AutoMapper;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.Interfaces;

namespace clothes_shop_api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ecommerceContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public UnitOfWork(ecommerceContext context, IMapper mapper, ITokenService tokenService)
        {
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public IAccountRepository AccountRepository => new AccountRepository(_context, _mapper, _tokenService);

        public IProductRepository ProductRepository => new ProductRepository(_context, _mapper);

        public ICartRepository CartRepository => new CartRepository(_context, _mapper);
        public IOrderRepository OrderRepository => new OrderRepository(_context, _mapper);
        public ICategoryRepository CategoryRepository => new CategoryRepository(_context, _mapper);
        public IColorRepository ColorRepository => new ColorRepository(_context, _mapper);

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanged()
        {
           return _context.ChangeTracker.HasChanges();
        }
    }
}
