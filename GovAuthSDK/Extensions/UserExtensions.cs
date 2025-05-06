using GovAuthSDK.DTO;
using GovAuthSDK.Models;

namespace GovAuthSDK.Extensions
{
    internal static class UserExtensions
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Login = user.Login,
                AccessLevel = user.AccessLevel,
            };
        }
    }
}
