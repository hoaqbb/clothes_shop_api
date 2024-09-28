using clothes_shop_api.DTOs.UserDtos;
using clothes_shop_api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRpository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRpository = accountRepository;
        }

        //[HttpPost("login")]
        //public async Task<AppUser> Login(LoginDto loginDto)
        //{

        //}

        //[HttpPost]
        //public async Task<ActionResult> Register(RegisterDto registerDto)
        //{
        //    if (await _accountRpository.IsUserExistedAsync(registerDto.Email)) return BadRequest("Username is taken!");

        //    using var hmac = new HMACSHA512();


        //}
    }
}
