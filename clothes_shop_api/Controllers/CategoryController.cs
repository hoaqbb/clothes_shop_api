using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.CategoryDtos;
using clothes_shop_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
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

        [HttpPost("create-category")]
        public async Task<ActionResult> CreateCategory2(CreateCategoryDto cat)
        {
            await _unitOfWork.CategoryRepository.AddAsync(new Category { Name = cat.Name });
            if (await _unitOfWork.SaveChangesAsync())
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut("update-category/{id}")]
        public async Task<ActionResult> UpdateCategory(int id, [FromBody] string value)
        {
            var cat = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (cat == null) return BadRequest();
            cat.Name = value;

            _unitOfWork.CategoryRepository.Update(cat);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return NoContent();
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var cat = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

            if(cat != null)
            {
                _unitOfWork.CategoryRepository.Delete(cat);
                if(await _unitOfWork.SaveChangesAsync())
                {
                    return NoContent();
                }
            }
            
            return BadRequest();
        }
    }
}
