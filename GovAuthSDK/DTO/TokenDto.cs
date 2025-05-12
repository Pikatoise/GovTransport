using GovAuthSDK.Enums;

namespace GovAuthSDK.DTO
{
    public class TokenDto
    {
        public string TokenValue { get; set; }
        public string Description { get; set; }
        public string End { get; set; }
        public AccessLevel AccessLevel { get; set; }
    }
}
