using GovAuthSDK.Enums;

namespace GovAuthSDK.Models
{
    public class User
    {
        public Guid Id { get; }

        public string Login { get; }

        public string PasswordHashed { get; }

        public AccessLevel AccessLevel { get; }

        public User()
        {

        }

        public User(string login, string passwordHashed, AccessLevel accessLevel)
        {
            Id = Guid.NewGuid();
            Login = login;
            PasswordHashed = passwordHashed;
            AccessLevel = accessLevel;
        }
    }
}
