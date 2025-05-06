namespace GovTransportSDK.Exceptions
{
    public class TransportNotFoundException: Exception
    {
        public TransportNotFoundException()
        {
        }

        public TransportNotFoundException(string vin) : base($"Transport with VIN '{vin}' was not found.")
        {
        }

        public TransportNotFoundException(Guid id) : base($"Transport with Id '{id}' was not found.")
        {

        }
    }
}
