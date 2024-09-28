using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.ProductDtos;
using clothes_shop_api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace clothes_shop_api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ecommerceContext _context;
        private readonly IMapper _mapper;

        public ProductRepository(ecommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ProductListDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .ProjectTo<ProductListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return products;
        }

        public async Task<ProductDetailDto> GetProductBySlugAsync(string slug)
        {
            var product = await _context.Products
                .Where(p => p.Slug == slug)
                .ProjectTo<ProductDetailDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            return product;
        }

        public async Task<IEnumerable<ProductListDto>> GetProductsByCategoryAsync(string category)
        {
            if(category == "all")
            {
                return await _context.Products
                   .ProjectTo<ProductListDto>(_mapper.ConfigurationProvider)
                   .ToListAsync();
            }
            if(category == "sale")
            {
                return await _context.Products
                   .Where(x => x.Discount > 0)
                   .ProjectTo<ProductListDto>(_mapper.ConfigurationProvider)
                   .ToListAsync();
            }

            return await _context.Products
                .Where(x => x.Category.Name == category)
                .ProjectTo<ProductListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
