using clothes_shop_api.DTOs.ColorDtos;

namespace clothes_shop_api.Interfaces
{
    public interface IColorRepository
    {
        Task<IEnumerable<ColorDto>> GetAllColorAsync();
        Task CreateColorAsync();
        Task DeleteColorAsync();
        Task UpdateColorAsync();
    }
}
