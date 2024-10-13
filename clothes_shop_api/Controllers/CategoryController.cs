using clothes_shop_api.DTOs.CategoryDtos;
using clothes_shop_api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("get-all-category")]
        public async Task<ActionResult<List<CategoryDto>>> GetAllCategory()
        {
            return Ok(await _unitOfWork.CategoryRepository.GetAllCategoryAsync());
        }
    }
}
