using GovAuthSDK.Enums;

namespace GovAuthSDK.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public AccessLevel AccessLevel { get; set; }
    }
}
