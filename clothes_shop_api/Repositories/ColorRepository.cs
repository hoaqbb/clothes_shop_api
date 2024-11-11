using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.ColorDtos;
using clothes_shop_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clothes_shop_api.Repositories
{
    public class ColorRepository : IColorRepository
    {
        private readonly ecommerce_decryptedContext _context;
        private readonly IMapper _mapper;

        public ColorRepository(ecommerce_decryptedContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ColorDto>> GetAllColorAsync()
        {
            return await _context.Colors
                .ProjectTo<ColorDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }
        public Task CreateColorAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteColorAsync()
        {
            throw new NotImplementedException();
        }

        

        public Task UpdateColorAsync()
        {
            throw new NotImplementedException();
        }
    }
}
