using clothes_shop_api.DTOs.ProductDtos;

namespace clothes_shop_api.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductListDto>> GetAllProductsAsync();
        Task<IEnumerable<ProductListDto>> GetProductsByCategoryAsync(string category);
        Task<ProductDetailDto> GetProductBySlugAsync(string slug);
    }
}
