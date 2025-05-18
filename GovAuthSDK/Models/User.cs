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

        public User(string login, string passwordHashed)
        {
            Id = Guid.NewGuid();
            AccessLevel = AccessLevel.High;
            Login = login;
            PasswordHashed = passwordHashed;
        }
    }
}
