using GovTransportSDK.Models;

namespace GovTransportSDK.DTO
{
    public class OwnerHistoryDetailedDto
    {
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public Ownership Owner { get; set; }
        public Transport Transport { get; set; }
    }
}
