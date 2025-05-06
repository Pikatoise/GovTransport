using GovTransportSDK.Enums;

namespace GovTransportSDK.Models
{
    public class Transport
    {
        public Guid Id { get; set; }

        public string VIN { get; set; }
        public string Model { get; set; }
        public int ReleaseYear { get; set; }
        public string Color { get; set; }
        public string GovNumber { get; set; }

        public TransportStatus Status { get; set; }
        public BodyType BodyType { get; set; }

        public IEnumerable<OwnerHistory> OwnerHistory { get; set; } = [];

        public Transport()
        {

        }

        public Transport(string vin, string model, int releaseYear, string color, string govNumber, TransportStatus status, BodyType type)
        {
            Id = Guid.NewGuid();

            VIN = vin;
            Model = model;
            ReleaseYear = releaseYear;
            Color = color;
            GovNumber = govNumber;
            Status = status;
            BodyType = type;
        }
    }
}
