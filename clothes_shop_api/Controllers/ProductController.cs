using clothes_shop_api.DTOs.ProductDtos;
using clothes_shop_api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductListDto>>> GetAllProducts()
        {
            return Ok(await _unitOfWork.ProductRepository.GetAllProductsAsync());
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
        public async Task<ActionResult<IEnumerable<ProductListDto>>> GetProductsByCategory(string category)
        {
            var products = await _unitOfWork.ProductRepository.GetProductsByCategoryAsync(category);

            if (products.IsNullOrEmpty())
                return NotFound("Product not found!");
            return Ok(products);
            
        }

    }
}
