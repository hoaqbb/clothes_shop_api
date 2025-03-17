using clothes_shop_api.Data.Entities;

namespace clothes_shop_api.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository AccountRepository { get; }
        IProductRepository ProductRepository { get; }
        ICartRepository CartRepository { get; }
        IOrderRepository OrderRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IColorRepository ColorRepository { get; }
        ISizeRepository SizeRepository { get; }
        Task<bool> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        bool HasChanged();
    }
}
