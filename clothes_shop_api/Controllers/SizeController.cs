using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.SizeDtos;
using clothes_shop_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SizeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SizeController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        // GET: api/<SizeController>
        [HttpGet("get-all-size")]
        public async Task<ActionResult<List<SizeDto>>> GetAllSize()
        {
            var sizes = await _unitOfWork.SizeRepository.GetAllAsync();
            return Ok(_mapper.Map<List<SizeDto>>(sizes));
        }

        // POST api/<SizeController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] string value)
        {
            if(string.IsNullOrEmpty(value)) return BadRequest();

            var newSize = new Size
            {
                Name = value
            };

            await _unitOfWork.SizeRepository.AddAsync(newSize);
            if (await _unitOfWork.SaveChangesAsync()) return CreatedAtRoute(null, _mapper.Map<SizeDto>(newSize));
            return BadRequest();
        }

        // PUT api/<SizeController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] string value)
        {
            if (string.IsNullOrEmpty(value)) return BadRequest();

            var size = await _unitOfWork.SizeRepository.GetByIdAsync(id);

            if (size == null) return BadRequest();

            size.Name = value;

            _unitOfWork.SizeRepository.Update(size);
            if (await _unitOfWork.SaveChangesAsync()) return NoContent();

            return BadRequest();
        }

        // DELETE api/<SizeController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var size = await _unitOfWork.SizeRepository.GetByIdAsync(id);

            if (size == null) return BadRequest();

            _unitOfWork.SizeRepository.Delete(size);
            if (await _unitOfWork.SaveChangesAsync()) return NoContent();

            return BadRequest();
        }
    }
}
