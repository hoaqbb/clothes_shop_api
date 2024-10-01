using clothes_shop_api.Data.Entities;
using clothes_shop_api.DTOs.UserDtos;

namespace clothes_shop_api.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
