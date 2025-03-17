using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.ColorDtos;
using clothes_shop_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clothes_shop_api.Repositories
{
    public class ColorRepository : GenericRepository<Color>, IColorRepository
    {
        private readonly ecommerce_decryptedContext _context;
        private readonly IMapper _mapper;

        public ColorRepository(ecommerce_decryptedContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ColorListDto>> GetAllColorAsync()
        {
            var colors = await _context.Colors
                .ProjectTo<ColorListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return colors;
        }
    }
}
