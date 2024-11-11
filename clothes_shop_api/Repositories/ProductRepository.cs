using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.ProductDtos;
using clothes_shop_api.Helpers;
using clothes_shop_api.Interfaces;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;

namespace clothes_shop_api.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ecommerce_decryptedContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public ProductRepository(ecommerce_decryptedContext context, IMapper mapper, IFileService fileService) : base(context) 
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<PagedList<ProductListDto>> GetAllProductsAsync(UserParams userParams)
        {
            var query = _context.Products
                .Where(p => p.IsVisible == true)
                .ProjectTo<ProductListDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

            query = userParams.SortBy switch
            {
                "price_ascending" => query.OrderBy(p => p.Price),
                "price_descending" => query.OrderByDescending(p => p.Price),
                "created_ascending" => query.OrderBy(p => p.CreateAt),
                _ => query.OrderByDescending(p => p.CreateAt),
            };

            return await PagedList<ProductListDto>.CreateAsync(
                query, 
                userParams.PageNumber, 
                userParams.PageSize);
        }

        public async Task<ProductDetailDto> GetProductBySlugAsync(string slug)
        {
            var product = await _context.Products
                .Where(p => p.Slug == slug && p.IsVisible == true)
                .ProjectTo<ProductDetailDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            return product;
        }

        public async Task<PagedList<ProductListDto>> GetProductsByCategoryAsync(UserParams userParams, string category, string role)
        {
            IQueryable<ProductListDto> query;
            if(role == "Admin")
            {
                query = _context.Products
                .ProjectTo<ProductListDto>(_mapper.ConfigurationProvider)
                .AsQueryable();
            } else
            {
                query = _context.Products
                .Where(x => x.IsVisible == true)
                .ProjectTo<ProductListDto>(_mapper.ConfigurationProvider)
                .AsQueryable();
            }

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
                        query = query.Where(x => x.Category == category);
                        break;
                    }
            }
            if(query.Any())
            {
                query = userParams.SortBy switch
                {
                    "created_ascending" => query.OrderBy(x => x.CreateAt),
                    "price_ascending" => query.OrderBy(x => x.Price),
                    "price_descending" => query.OrderByDescending(x => x.Price),
                    _ => query.OrderByDescending(x => x.CreateAt)
                };
            }

            return await PagedList<ProductListDto>.CreateAsync(
                query.AsNoTracking(),
                userParams.PageNumber,
                userParams.PageSize
                );
        }

        public async Task CreateProductAsync(CreateProductDto createProductDto)
        {
            //try
            //{
                var createProduct = new Product
                {
                    Name = createProductDto.Name,
                    Slug = createProductDto.Slug,
                    Price = createProductDto.Price,
                    Description = createProductDto.Description,
                    Discount = createProductDto.Discount,
                    CategoryId = createProductDto.CategoryId
                };
                //await _context.Database.BeginTransactionAsync();
                await _context.Products.AddAsync(createProduct);
                await _context.SaveChangesAsync();

                var s = new List<ProductColor>();
                foreach (var item in createProductDto.ProductColors) 
                {
                    var c = await _context.ProductColors.AddAsync(new ProductColor
                    {
                        ProductId = createProduct.Id,
                        ColorId = item,
                    });
                    
                    s.Add(c.Entity);    
                }
                await _context.SaveChangesAsync();
                foreach (var item in createProductDto.ProductSizes)
                {
                    foreach (var item1 in s)
                    {
                        await _context.Quantities.AddAsync(new Quantity
                        {
                            ProductId = createProduct.Id,
                            ProductColorId = item1.Id,
                            SizeId = item,
                        });
                    }
                }

                var mainImgResult = await _fileService.AddImageAsync(createProductDto.MainImage);
                if(mainImgResult.Error == null)
                {
                    _context.Add(new ProductImage
                    {
                        ProductId = createProduct.Id,
                        IsMain = true,
                        ImageUrl = mainImgResult.SecureUrl.AbsoluteUri,
                        PublicId = mainImgResult.PublicId
                    });
                }

                var subImgResult = await _fileService.AddImageAsync(createProductDto.SubImage);
                if (subImgResult.Error == null)
                {
                    _context.Add(new ProductImage
                    {
                        ProductId = createProduct.Id,
                        IsSub = true,
                        ImageUrl = subImgResult.SecureUrl.AbsoluteUri,
                        PublicId = subImgResult.PublicId
                    });
                }

                var otherImgResults = await _fileService.AddMultipleImageAsync(createProductDto.ProductImages);
                foreach (var item in otherImgResults)
                {
                    if (item.Error == null)
                    {
                        var image = new ProductImage
                        {
                            ImageUrl = item.SecureUrl.AbsoluteUri,
                            PublicId = item.PublicId,
                            ProductId = createProduct.Id
                        };

                        _context.Add(image);
                    }
                }

                //await _context.SaveChangesAsync();
                //await _context.Database.CommitTransactionAsync();

            //} catch
            //{
            //    await _context.Database.RollbackTransactionAsync();
            //    return false;
            //}
            
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            var productImages = await _context.ProductImages
                .Include(x => x.Product)
                .Where(x => x.ProductId == product.Id)
                .ToListAsync();
            try
            {
                await _context.Database.BeginTransactionAsync();
                if (productImages != null)
                {
                    foreach (var image in productImages)
                    {
                        if (image.PublicId != null)
                        {
                            var result = await _fileService.DeleteImageAsync(image.PublicId);
                            if (result.Error != null) break;
                        }
                    }
                }
                _context.Remove(product);
                if (await _context.SaveChangesAsync() > 0)
                {
                    await _context.Database.CommitTransactionAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                await _context.Database.RollbackTransactionAsync();
                return false;
            }
        }

        public Task AddProductImageAsync(int id, IFormFile file)
        {
            throw new NotImplementedException();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColors)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> UpdateProduct(UpdateProductDto updateProductDto)
        {
            var product = await _context.Products
                .Include(p => p.ProductColors)
                .Include(p => p.Quantities)
                .FirstOrDefaultAsync(x => x.Id == updateProductDto.Id);

            if(product is null) return false;

            try
            {
                await _context.Database.BeginTransactionAsync();
                product.Name = updateProductDto.Name;
                product.Slug = updateProductDto.Slug;
                product.Price = updateProductDto.Price;
                product.Discount = updateProductDto.Discount;
                //product.IsVisible = updateProductDto.IsVisible;
                product.CategoryId = updateProductDto.CategoryId;
                product.Description = updateProductDto.Description;
                product.UpdateAt = DateTime.Now;

                //var existingProductColors = product.ProductColors
                //    .Where(c => c.ProductId == product.Id)
                //    .ToList();

                //var mergedProductColors = new List<ProductColor>();
                //foreach (var item in existingProductColors)
                //{
                //    if (updateProductDto.ProductColors.Contains(item.ColorId))
                //        mergedProductColors.Add(item);
                //}
                
                //foreach (var item in updateProductDto.ProductColors)
                //{
                //    if (!mergedProductColors.Any(p => p.ColorId == item))
                //    {
                //        mergedProductColors.Add(new ProductColor
                //        {
                //            ProductId = product.Id,
                //            ColorId = item
                //        });
                //    }
                    
                //}

                //_context.ProductColors.AddRange(mergedProductColors);

                //var sizeQuery = product.Quantities
                //    .Where(c => c.ProductId == product.Id)
                //    .AsQueryable();
                //foreach (var item in updateProductDto.ProductSizes)
                //{
                //    if (await sizeQuery.FirstOrDefaultAsync(x => x.SizeId == item) is null) continue;
                //    var quantity = new Quantity
                //    {
                //        ProductId = product.Id,
                //        SizeId = item,
                //        Amount = 0
                //    };
                //    product.Quantities.Add(quantity);
                //}

                _context.Products.Update(product);
                if (await _context.SaveChangesAsync() > 0)
                {
                    await _context.Database.CommitTransactionAsync();
                    return true;
                }
                    
                return false;
            }
            catch
            {
                await _context.Database.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<bool> UpdateProductStatusAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product is null) return false;

            product.IsVisible = !product.IsVisible;
            _context.Update(product);

            return true;
        }
    }
}
