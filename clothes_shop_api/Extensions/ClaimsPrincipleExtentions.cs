using System.Security.Claims;

namespace clothes_shop_api.Extensions
{
    public static class ClaimsPrincipleExtentions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return -1;
            }
            return int.Parse(userIdClaim);
        }
        //return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    
        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string GetRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
