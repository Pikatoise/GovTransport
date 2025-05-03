namespace GovAuthSDK.Exceptions
{
    public class UserNotFoundException: Exception
    {
        public UserNotFoundException()
        {
        }

        public UserNotFoundException(string login) : base($"User with login '{login}' was not found")
        {
        }

        public UserNotFoundException(Guid id) : base($"User with Id '{id}' was not found")
        {
        }
    }
}
