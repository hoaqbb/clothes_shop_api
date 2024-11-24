using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.ProductDtos;
using clothes_shop_api.Helpers;

namespace clothes_shop_api.Interfaces
{
    public interface IProductRepository
    {
        Task<PagedList<ProductListDto>> GetProductsAsync(UserProductParams userParams);
        Task<PagedList<ProductListDto>> GetAllProductsAsync(AdminProductParams adminProductParams);
        Task<ProductDetailDto> GetProductBySlugAsync(string slug);
        Task<ProductDetailDto> GetProductByIdAsync(int id);
        Task CreateProductAsync(CreateProductDto createProductDto);
        Task<bool> UpdateProduct(UpdateProductDto updateProductDto);
        Task<bool> UpdateProductStatusAsync(int id);
        Task<bool> DeleteProduct(int id);
    }
}
