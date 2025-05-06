namespace GovTransportSDK.DTO
{
    public class AddOwnershipDto
    {
        public string FullName { get; set; }
        public string RegistrationAddress { get; set; }
        public string Passport { get; set; }

        public string Osago { get; set; } = string.Empty;
        public bool IsLegal { get; set; } = false;
    }
}
