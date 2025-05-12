namespace GovAuthSDK.Exceptions
{
    public class TokenNotFoundException: Exception
    {
        public TokenNotFoundException()
        {
        }

        public TokenNotFoundException(Guid id) : base($"Token with id '{id}' not found.")
        {
        }

        public TokenNotFoundException(string token) : base($"Token '{token}' was not found.")
        {
        }
    }
}
