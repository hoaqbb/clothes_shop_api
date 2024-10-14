using AutoMapper;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.ProductDtos;
using clothes_shop_api.DTOs.ProductImageDtos;
using clothes_shop_api.Extensions;
using clothes_shop_api.Helpers;
using clothes_shop_api.Interfaces;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly ecommerceContext _context;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IFileService fileService, ecommerceContext context, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductListDto>>> GetAllProducts([FromQuery]UserParams userParams)
        {
            return Ok(await _unitOfWork.ProductRepository.GetAllProductsAsync(userParams));
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<ProductDetailDto>> GetProductBySlug(string slug)
        {
            var product = await _unitOfWork.ProductRepository.GetProductBySlugAsync(slug);

            if (product != null)
                return Ok(product);
            return NotFound("Product not found!");
        }

        [HttpGet("categories/{category}")]
        public async Task<ActionResult<IEnumerable<ProductListDto>>> GetProductsByCategory([FromQuery]UserParams userParams ,string category)
        {
            var products = await _unitOfWork.ProductRepository.GetProductsByCategoryAsync(userParams, category);

            Response.AddPaginationHeader(products.CurrentPage, products.PageSize, 
                products.TotalCount, products.TotalPages);
            //if (products.IsNullOrEmpty())
            //    return NotFound("Product not found!");
            return Ok(products);
            
        }

        [HttpPost("create-product")]
        public async Task<ActionResult> CreateProduct(CreateProductDto createProduct)
        {
            if(await _unitOfWork.ProductRepository.CreateProductAsync(createProduct))
                return Ok();
            return BadRequest();
        }

        [HttpPut("update-product")]
        public async Task<ActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {
            if (await _unitOfWork.ProductRepository.UpdateProduct(updateProductDto))
                return Ok();
            return BadRequest();
        }

        [HttpDelete("delete-product/{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            if(await _unitOfWork.ProductRepository.DeleteProduct(id))
                return Ok();
            return BadRequest();
        }

        [HttpPost("add-product-images")]
        public async Task<ActionResult<List<ProductImageDto>>> AddProductImages([FromForm]IFormFile[] files, [FromQuery]int id)
        {
            var product = await _unitOfWork.ProductRepository.GetProductByIdAsync(id);
            var result = new List<ImageUploadResult>();

            try
            {
                await _context.Database.BeginTransactionAsync();
                foreach (var file in files)
                {
                    result.Add(await _fileService.AddImageAsync(file));
                }
                var listProductImages = new List<ProductImageDto>();
                foreach (var item in result)
                {
                    if (item.Error != null)
                    {
                        await _context.Database.RollbackTransactionAsync();
                        return BadRequest(item.Error.Message);
                    }
                        
                    var image = new ProductImage
                    {
                        ImageUrl = item.SecureUrl.AbsoluteUri,
                        PublicId = item.PublicId,
                        ProductId = id
                    };

                    if (product.ProductImages.Count == 0)
                    {
                        image.IsMain = true;
                    }
                    if (product.ProductImages.Count == 1)
                    {
                        image.IsSub = true;
                    }

                    product.ProductImages.Add(image);
                    _context.SaveChanges();

                    listProductImages.Add(_mapper.Map<ProductImageDto>(image));
                }
                
                if (listProductImages.Count() > 0)
                {
                    await _context.Database.CommitTransactionAsync();
                    return Ok(listProductImages);
                }
                return BadRequest("Problem adding image!");
            }
            catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("delete-image/{productId}/{imageId}")]
        public async Task<ActionResult> DeleteImage(int productId, int imageId)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == productId);

            var image = await _context.ProductImages
                .Include(x => x.Product)
                .Where(x => x.Id == imageId && x.Product.Id == productId)
                .SingleOrDefaultAsync(x => x.Id == imageId);

            if (image is null) return NotFound();

            if (image.IsMain) return BadRequest("You cannot delete your main photo!");
            if (image.IsSub) return BadRequest("You cannot delete your sub photo!");

            if (image.PublicId != null)
            {
                var result = await _fileService.DeleteImageAsync(image.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            product.ProductImages.Remove(image);

            if (await _unitOfWork.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete the image!");
        }

        [HttpPut("set-main-image/{productId}")]
        public async Task<ActionResult> SetMainImage(int productId, [FromQuery]int imageId)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == productId);

            var image = product.ProductImages.FirstOrDefault(x => x.Id == imageId);

            if (image.IsMain) return BadRequest("This is already main image");

            var currentMain = product.ProductImages.FirstOrDefault(x => x.IsMain);

            if (currentMain != null) currentMain.IsMain = false;
            image.IsMain = true;

            if (await _unitOfWork.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [HttpPut("set-sub-image/{productId}")]
        public async Task<ActionResult> SetSubImage(int productId, [FromQuery]int imageId)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == productId);

            if (product is null) return NotFound();

            var image = product.ProductImages.FirstOrDefault(x => x.Id == imageId);

            if (image.IsSub) return BadRequest("This is already sub image");

            var currentSub = product.ProductImages.FirstOrDefault(x => x.IsSub);

            if (currentSub != null) currentSub.IsSub = false;
            image.IsSub = true;

            if (await _unitOfWork.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to set main photo");
        }
    }
}
