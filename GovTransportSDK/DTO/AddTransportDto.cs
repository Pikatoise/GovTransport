using GovTransportSDK.Enums;

namespace GovTransportSDK.DTO
{
    public class AddTransportDto
    {
        public string VIN { get; set; }
        public string Model { get; set; }
        public int ReleaseYear { get; set; }
        public string Color { get; set; }

        public string RegionCode { get; set; }

        public TransportStatus Status { get; set; }
        public BodyType BodyType { get; set; }
    }
}
