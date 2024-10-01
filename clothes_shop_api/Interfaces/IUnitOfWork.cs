namespace clothes_shop_api.Interfaces
{
    public interface IUnitOfWork
    {
        IAccountRepository AccountRepository { get; }
        IProductRepository ProductRepository { get; }
        Task<bool> SaveAllAsync();
        bool HasChanged();
    }
}
