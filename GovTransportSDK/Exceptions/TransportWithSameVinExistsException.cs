namespace GovTransportSDK.Exceptions
{
    public class TransportWithSameVinExistsException: Exception
    {
        public TransportWithSameVinExistsException()
        {
        }

        public TransportWithSameVinExistsException(string vin) : base($"Transport with VIN '{vin}' already exists.")
        {
        }
    }
}
