using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.SizeDtos;
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
        private readonly ecommerceContext _context;
        private readonly IMapper _mapper;

        public SizeController(ecommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        // GET: api/<SizeController>
        [HttpGet("get-all-size")]
        public async Task<ActionResult<List<SizeDto>>> GetAllSize()
        {
            return Ok(await _context.Sizes
                .ProjectTo<SizeDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync());
        }

        // GET api/<SizeController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SizeController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SizeController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SizeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
