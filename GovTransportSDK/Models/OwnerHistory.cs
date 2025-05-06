namespace GovTransportSDK.Models
{
    public class OwnerHistory
    {
        public Guid Id { get; set; }
        public Guid TransportId { get; set; }
        public Guid OwnershipId { get; set; }

        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        public Transport Transport { get; set; }
        public Ownership Ownership { get; set; }

        public OwnerHistory()
        {

        }

        public OwnerHistory(Guid transportId, Guid ownershipId, DateTime start, DateTime? end)
        {
            Id = Guid.NewGuid();

            TransportId = transportId;
            OwnershipId = ownershipId;
            Start = start;
            End = end;
        }
    }
}
