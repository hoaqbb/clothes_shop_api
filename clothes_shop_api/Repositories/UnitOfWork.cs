using AutoMapper;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace clothes_shop_api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ecommerce_decryptedContext _context;
        private IDbContextTransaction _transaction;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IFileService _fileService;
        private readonly ICacheService _cacheService;

        public UnitOfWork(ecommerce_decryptedContext context, IMapper mapper, ITokenService tokenService, IFileService fileService, ICacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
            _fileService = fileService;
            _cacheService = cacheService;
        }
        public IAccountRepository AccountRepository => new AccountRepository(_context, _mapper, _tokenService);

        public IProductRepository ProductRepository => new ProductRepository(_context, _mapper, _fileService);

        public ICartRepository CartRepository => new CartRepository(_context, _mapper, _cacheService);
        public IOrderRepository OrderRepository => new OrderRepository(_context, _mapper);
        public ICategoryRepository CategoryRepository => new CategoryRepository(_context, _mapper);
        public IColorRepository ColorRepository => new ColorRepository(_context, _mapper);

        public bool HasChanged()
        {
           return _context.ChangeTracker.HasChanges();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw new Exception();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null!;
            }
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }
    }
}
