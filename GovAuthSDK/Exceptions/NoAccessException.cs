namespace GovAuthSDK.Exceptions
{
    public class NoAccessException: Exception
    {
        public NoAccessException()
        {
        }

        public NoAccessException(string minimalAccessLevel) : base($"Your access level is to low. Minimal level for this request is '{minimalAccessLevel}'.")
        {
        }
    }
}
