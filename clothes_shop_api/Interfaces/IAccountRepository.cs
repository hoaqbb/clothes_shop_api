using clothes_shop_api.DTOs.UserDtos;

namespace clothes_shop_api.Interfaces
{
    public interface IAccountRepository
    {
        Task<AppUser> AuthenticateAsync(LoginDto loginDto);
        Task<AppUser> RegisterAsync(RegisterDto registerDto);
        Task<bool> IsUserExistedAsync(string email);
    }
}
