namespace GovTransportSDK.Exceptions
{
    public class OwnerWithSamePassportExistsException: Exception
    {
        public OwnerWithSamePassportExistsException()
        {
        }

        public OwnerWithSamePassportExistsException(string passport) : base($"Ownership with passport '{passport}' already exists.")
        {
        }
    }
}
