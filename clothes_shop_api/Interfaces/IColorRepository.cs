using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.ColorDtos;
using clothes_shop_api.Repositories;
using System.Collections.Generic;

namespace clothes_shop_api.Interfaces
{
    public interface IColorRepository : IGenericRepository<Color>
    {
        Task<IEnumerable<ColorListDto>> GetAllColorAsync();
    }
}
