namespace GovTransportSDK.Exceptions
{
    public class OwnerNotFoundException: Exception
    {
        public OwnerNotFoundException()
        {
        }

        public OwnerNotFoundException(Guid id) : base($"Owner with Id '{id}' was not found.")
        {
        }
    }
}
