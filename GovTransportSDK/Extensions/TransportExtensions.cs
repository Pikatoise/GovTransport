using GovTransportSDK.DTO;
using GovTransportSDK.Models;

namespace GovTransportSDK.Extensions
{
    public static class TransportExtensions
    {
        public static TransportDto ToDto(this Transport transport)
        {
            return new TransportDto()
            {
                VIN = transport.VIN,
                Model = transport.Model,
                ReleaseYear = transport.ReleaseYear,
                Color = transport.Color,
                GovNumber = transport.GovNumber,
                Status = transport.Status,
                BodyType = transport.BodyType
            };
        }
    }
}
