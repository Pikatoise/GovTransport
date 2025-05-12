using GovAuthSDK.DTO;
using GovAuthSDK.Helpers;
using GovAuthSDK.Models;
using System.Security.Claims;

namespace GovAuthSDK.Extensions
{
    internal static class TokenExtensions
    {
        public static TokenDto ToDto(this Token token)
        {
            return new TokenDto()
            {
                TokenValue = token.AuthToken,
                AccessLevel = token.AccessLevel,
                End = JwtHelper.ValidateToken(token.AuthToken).Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Version)).Value,
                Description = JwtHelper.ValidateToken(token.AuthToken).Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name)).Value
            };
        }
    }
}
