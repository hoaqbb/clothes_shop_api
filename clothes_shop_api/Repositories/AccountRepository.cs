﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.UserDtos;
using clothes_shop_api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Security.Cryptography;
using System.Text;

namespace clothes_shop_api.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ecommerce_decryptedContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public AccountRepository(ecommerce_decryptedContext context, IMapper mapper, ITokenService tokenService)
        {
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
        }
        public async Task<UserDto> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.Email == loginDto.Email);

            if (user is null) return null;

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return null;
            }
            
            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            };
        }

        public async Task<UserDetailDto> GetUserDetailAsync(int id)
        {
            var user = await _context.Users
                .Where(x => x.Id == id)
                .ProjectTo<UserDetailDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
            return user;
        }

        public Task<bool> IsUserExistedAsync(string email)
        {
            return _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            using var hmac = new HMACSHA512();

            var user = _mapper.Map<User>(registerDto);

            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var userCart = new Cart
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id
            };
            _context.Add(userCart);

            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}
