namespace GovAuthSDK.Exceptions
{
    public class WrongPasswordException: Exception
    {
        public WrongPasswordException()
        {
        }

        public WrongPasswordException(string? message) : base("Wrong password.")
        {
        }
    }
}
