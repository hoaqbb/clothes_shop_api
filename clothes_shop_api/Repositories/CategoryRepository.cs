using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.CategoryDtos;
using clothes_shop_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clothes_shop_api.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ecommerceContext _context;
        private readonly IMapper _mapper;

        public CategoryRepository(ecommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoryAsync()
        {
            var categories = await _context.Categories
                .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            return categories;
        }
        public Task CreateCategoryAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteCategoryAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateCategoryAsync()
        {
            throw new NotImplementedException();
        }
    }
}
