using GovAuthSDK.Enums;
using GovAuthSDK.Helpers;

namespace GovAuthSDK.Models
{
    public class Token
    {
        public Guid Id { get; }

        public string AuthToken { get; }

        public AccessLevel AccessLevel { get; }

        public Token()
        {

        }

        public Token(string owner, DateTime cancellation)
        {
            Id = Guid.NewGuid();
            AuthToken = JwtHelper.GenerateToken(owner, cancellation);
            AccessLevel = AccessLevel.Medium;
        }
    }
}
