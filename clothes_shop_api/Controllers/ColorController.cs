using AutoMapper;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.ColorDtos;
using clothes_shop_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ecommerce_decryptedContext _context;

        public ColorController(IUnitOfWork unitOfWork, IMapper mapper, ecommerce_decryptedContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("get-all-color")]
        public async Task<ActionResult> GetAllColor()
        {
            var colors = await _unitOfWork.ColorRepository.GetAllColorAsync();
            return Ok(colors);
        }

        // POST api/<ValuesController>
        [HttpPost]
        public async Task<ActionResult> CreateColor(ColorDto colorDto)
        {
            var color = new Color
            {
                Name = colorDto.Name,
                ColorCode = colorDto.ColorCode
            };
            await _unitOfWork.ColorRepository.AddAsync(color);

            if(await _unitOfWork.SaveChangesAsync()) 
                return Ok(_mapper.Map<ColorListDto>(color));

            return BadRequest();
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, ColorDto colorDto)
        {
            
            var color = await _unitOfWork.ColorRepository.GetByIdAsync(id);

            if(color == null) return BadRequest();

            color.Name = colorDto.Name;
            color.ColorCode = colorDto.ColorCode;
            _unitOfWork.ColorRepository.Update(color);
            if(await _unitOfWork.SaveChangesAsync()) return NoContent();
            
            return BadRequest();
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var color = await _unitOfWork.ColorRepository.GetByIdAsync(id);
            if(color == null) return BadRequest();

            _unitOfWork.ColorRepository.Delete(color);
            if (await _unitOfWork.SaveChangesAsync()) return NoContent();

            return BadRequest();
        }
    }
}
