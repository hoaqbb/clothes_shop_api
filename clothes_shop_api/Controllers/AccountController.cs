using AutoMapper;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.UserDtos;
using clothes_shop_api.Extensions;
using clothes_shop_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace clothes_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _unitOfWork.AccountRepository.AuthenticateAsync(loginDto);
            if (user is null) return BadRequest("Invalid email or password!");

            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await _unitOfWork.AccountRepository.IsUserExistedAsync(registerDto.Email)) 
                return BadRequest("Email is taken!");

            return Ok(await _unitOfWork.AccountRepository.RegisterAsync(registerDto));
        }

        [Authorize]
        [HttpGet("get-user-detail")]
        public async Task<ActionResult<UserDetailDto>> GetUserDetailAsync()
        {
            var userId = User.GetUserId();
            var user = await _unitOfWork.AccountRepository.GetUserDetailAsync(userId);
            if (user is null) return Unauthorized();

            return Ok(user);
        }
    }
}
