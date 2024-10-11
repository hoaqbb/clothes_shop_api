using clothes_shop_api.DTOs.UserDtos;

namespace clothes_shop_api.Interfaces
{
    public interface IAccountRepository
    {
        Task<UserDto> AuthenticateAsync(LoginDto loginDto);
        Task<UserDto> RegisterAsync(RegisterDto registerDto);
        Task<bool> IsUserExistedAsync(string email);
        Task<UserDetailDto> GetUserDetailAsync(int id);
    }
}
