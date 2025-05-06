using GovTransportSDK.Enums;

namespace GovTransportSDK.DTO
{
    public class TransportDto
    {
        public string VIN { get; set; }
        public string Model { get; set; }
        public int ReleaseYear { get; set; }
        public string Color { get; set; }
        public string GovNumber { get; set; }
        public TransportStatus Status { get; set; }
    }
}
