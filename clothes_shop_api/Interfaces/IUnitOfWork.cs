namespace clothes_shop_api.Interfaces
{
    public interface IUnitOfWork
    {
        IAccountRepository AccountRepository { get; }
        IProductRepository ProductRepository { get; }
        ICartRepository CartRepository { get; }
        IOrderRepository OrderRepository { get; }
        Task<bool> SaveAllAsync();
        bool HasChanged();
    }
}
