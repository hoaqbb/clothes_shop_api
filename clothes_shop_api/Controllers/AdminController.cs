using clothes_shop_api.DTOs.OrderDtos;
using clothes_shop_api.DTOs.ProductDtos;
using clothes_shop_api.Extensions;
using clothes_shop_api.Helpers;
using clothes_shop_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("get-all-product")]
        public async Task<ActionResult<IEnumerable<ProductListDto>>> GetAllProduct([FromQuery] AdminProductParams adminProductParams)
        {
            var productList = await _unitOfWork.ProductRepository.GetAllProductsAsync(adminProductParams);

            Response.AddPaginationHeader(productList.CurrentPage, productList.PageSize,
                productList.TotalCount, productList.TotalPages);

            return Ok(productList);
        }

        [HttpGet("get-all-order")]
        public async Task<ActionResult<List<OrderListDto>>> GetAllOrder([FromQuery] AdminOrderParams paginationParams)
        {
            var orderList = await _unitOfWork.OrderRepository.GetAllOrderAsync(paginationParams);

            Response.AddPaginationHeader(orderList.CurrentPage, orderList.PageSize,
                orderList.TotalCount, orderList.TotalPages);

            return Ok(orderList);
        }

        [HttpGet("get-product-detail/{id}")]
        public async Task<ActionResult<ProductDetailDto>> GetProductDetailById(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetProductByIdAsync(id);

            if (product != null)
                return Ok(product);
            return NotFound("Product not found!");
        }
    }
}
