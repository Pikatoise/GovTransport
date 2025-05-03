using GovAuthSDK.DTO;
using GovAuthSDK.Helpers;
using GovAuthSDK.Models;
using System.Security.Claims;

namespace GovAuthSDK.Extensions
{
    public static class TokenExtensions
    {
        public static TokenDto ToDto(this Token token)
        {
            return new TokenDto()
            {
                AccessLevel = token.AccessLevel,
                Description = JwtHelper.ValidateToken(token.AuthToken).Claims.FirstOrDefault(x => x.ValueType.Equals(ClaimTypes.Name)).Value
            };
        }
    }
}
