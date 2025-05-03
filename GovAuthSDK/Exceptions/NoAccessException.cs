namespace GovAuthSDK.Exceptions
{
    public class NoAccessException: Exception
    {
        public NoAccessException()
        {
        }

        public NoAccessException(string? message) : base("You dont have access.")
        {
        }
    }
}
