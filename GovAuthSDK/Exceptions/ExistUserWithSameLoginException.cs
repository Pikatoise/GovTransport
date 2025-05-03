namespace GovAuthSDK.Exceptions
{
    public class ExistUserWithSameLoginException: Exception
    {
        public ExistUserWithSameLoginException()
        {
        }

        public ExistUserWithSameLoginException(string login) : base($"User with login {login} already exists.")
        {
        }
    }
}
