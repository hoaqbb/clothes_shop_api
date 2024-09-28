using clothes_shop_api.DTOs.ProductDtos;
using clothes_shop_api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository) 
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductListDto>>> GetAllProducts()
        {
            return Ok(await _productRepository.GetAllProductsAsync());
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<ProductDetailDto>> GetProductBySlug(string slug)
        {
            var product = await _productRepository.GetProductBySlugAsync(slug);

            if (product != null)
                return Ok(product);
            return BadRequest("Product not found!");
        }

        [HttpGet("categories/{category}")]
        public async Task<ActionResult<IEnumerable<ProductListDto>>> GetProductsByCategory(string category)
        {
            var products = await _productRepository.GetProductsByCategoryAsync(category);

            if (products != null)
                return Ok(products);
            return NotFound("Product not found!");
        }

    }
}
