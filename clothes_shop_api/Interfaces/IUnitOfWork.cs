namespace clothes_shop_api.Interfaces
{
    public interface IUnitOfWork
    {
        IAccountRepository AccountRepository { get; }
        IProductRepository ProductRepository { get; }
        ICartRepository CartRepository { get; }
        Task<bool> SaveAllAsync();
        bool HasChanged();
    }
}
