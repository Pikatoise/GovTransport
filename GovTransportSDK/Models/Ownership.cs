namespace GovTransportSDK.Models
{
    public class Ownership
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string RegistrationAddress { get; set; }
        public string Passport { get; set; }

        public string Osago { get; set; } = string.Empty;
        public bool IsLegal { get; set; } = false;

        public ICollection<OwnerHistory> TransportsHistory { get; set; } = new List<OwnerHistory>();

        public Ownership()
        {

        }

        public Ownership(string fullName, string registrationAddress, string passport, string osago, bool isLegal = false)
        {
            Id = Guid.NewGuid();
            FullName = fullName;
            RegistrationAddress = registrationAddress;
            Passport = passport;
            Osago = osago;
            IsLegal = isLegal;
        }
    }
}
