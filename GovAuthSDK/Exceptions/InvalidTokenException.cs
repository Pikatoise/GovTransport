namespace GovAuthSDK.Exceptions
{
    public class InvalidTokenException: Exception
    {
        public InvalidTokenException()
        {
        }

        public InvalidTokenException(string token) : base($"Token '{token}' is invalid or not found.")
        {
        }
    }
}
