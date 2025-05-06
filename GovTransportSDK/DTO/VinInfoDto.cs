namespace GovTransportSDK.DTO
{
    public class VinInfoDto
    {
        public TransportDto Transport { get; set; }
        public IEnumerable<OwnerHistoryMinimizedDto> History { get; set; }
    }
}
