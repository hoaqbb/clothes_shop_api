using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.UserDtos;
using clothes_shop_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clothes_shop_api.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ecommerceContext _context;

        public AccountRepository(ecommerceContext context)
        {
            _context = context;
        }
        public Task<AppUser> AuthenticateAsync(LoginDto loginDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserExistedAsync(string email)
        {
            return _context.Users.AnyAsync(u => u.Email == email);
        }

        public Task<AppUser> RegisterAsync(RegisterDto registerDto)
        {
            throw new NotImplementedException();
        }
    }
}
