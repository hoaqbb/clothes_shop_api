using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.CategoryDtos;

namespace clothes_shop_api.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoryAsync();
    }
}
