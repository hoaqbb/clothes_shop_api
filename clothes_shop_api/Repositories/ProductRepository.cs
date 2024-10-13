using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.ProductDtos;
using clothes_shop_api.Helpers;
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

        public async Task<PagedList<ProductListDto>> GetAllProductsAsync(UserParams userParams)
        {
            var query = _context.Products.AsQueryable();

            query = userParams.SortBy switch
            {
                "price_ascending" => query.OrderBy(p => p.Price),
                "price_descending" => query.OrderByDescending(p => p.Price),
                "created_ascending" => query.OrderBy(p => p.CreateAt),
                _ => query.OrderByDescending(p => p.CreateAt),
            };

            return await PagedList<ProductListDto>.CreateAsync(
                query.ProjectTo<ProductListDto>(_mapper.ConfigurationProvider), 
                userParams.PageNumber, 
                userParams.PageSize);
        }

        public async Task<ProductDetailDto> GetProductBySlugAsync(string slug)
        {
            var product = await _context.Products
                .Where(p => p.Slug == slug)
                .ProjectTo<ProductDetailDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            return product;
        }

        public async Task<PagedList<ProductListDto>> GetProductsByCategoryAsync(UserParams userParams, string category)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            switch (category) 
            {
                case "sale":
                    {
                        query = query.Where(x => x.Discount > 0);
                        break;
                    }
                case "all":
                    {
                        break;
                    }
                default:
                    {
                        query = query.Where(x => x.Category.Name == category);
                        break;
                    }
            }
            if(query.Any())
            {
                query = userParams.SortBy switch
                {
                    "created_ascending" => query.OrderByDescending(x => x.CreateAt),
                    "price_ascending" => query.OrderBy(x => x.Price),
                    "price_descending" => query.OrderByDescending(x => x.Price),
                    _ => query.OrderBy(x => x.CreateAt)
                };
            }

            return await PagedList<ProductListDto>.CreateAsync(
                query.ProjectTo<ProductListDto>(_mapper.ConfigurationProvider).AsNoTracking(),
                userParams.PageNumber,
                userParams.PageSize
                );
        }

        public async Task CreateProductAsync(CreateProductDto createProductDto)
        {
            try
            {
                var createProduct = new Product
                {
                    Name = createProductDto.Name,
                    Slug = createProductDto.Slug,
                    Price = createProductDto.Price,
                    Description = createProductDto.Description,
                    Discount = createProductDto.Discount,
                    CategoryId = createProductDto.CategoryId
                };
                await _context.Database.BeginTransactionAsync();
                await _context.Products.AddAsync(createProduct);


                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
            } catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
            }
            
        }
    }
}
