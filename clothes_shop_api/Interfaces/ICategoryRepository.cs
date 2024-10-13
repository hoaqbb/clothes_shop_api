using clothes_shop_api.DTOs.CategoryDtos;

namespace clothes_shop_api.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoryAsync();
        Task CreateCategoryAsync();
        Task DeleteCategoryAsync();
        Task UpdateCategoryAsync();
    }
}
