using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.ProductDtos;
using clothes_shop_api.Helpers;

namespace clothes_shop_api.Interfaces
{
    public interface IProductRepository
    {
        Task<PagedList<ProductListDto>> GetAllProductsAsync(UserParams userParams);
        Task<PagedList<ProductListDto>> GetProductsByCategoryAsync(UserParams userParams, string category, string role);
        Task<ProductDetailDto> GetProductBySlugAsync(string slug);
        Task<Product> GetProductByIdAsync(int id);
        Task CreateProductAsync(CreateProductDto createProductDto);
        Task AddProductImageAsync(int id, IFormFile file);
        Task<bool> UpdateProduct(UpdateProductDto updateProductDto);
        Task<bool> UpdateProductStatusAsync(int id);
        Task<bool> DeleteProduct(int id);
    }
}
