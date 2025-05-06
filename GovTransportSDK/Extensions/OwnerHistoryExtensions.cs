using GovTransportSDK.DTO;
using GovTransportSDK.Models;

namespace GovTransportSDK.Extensions
{
    public static class OwnerHistoryExtensions
    {
        public static OwnerHistoryMinimizedDto ToMinimizedDto(this OwnerHistory ownerHistory)
        {
            return new OwnerHistoryMinimizedDto()
            {
                Start = ownerHistory.Start,
                End = ownerHistory.End,
                IsLegal = ownerHistory.Ownership.IsLegal
            };
        }

        public static OwnerHistoryDetailedDto ToDetailedDto(this OwnerHistory ownerHistory)
        {
            return new OwnerHistoryDetailedDto()
            {
                Start = ownerHistory.Start,
                End = ownerHistory.End,
                Owner = ownerHistory.Ownership,
                Transport = ownerHistory.Transport
            };
        }
    }
}
