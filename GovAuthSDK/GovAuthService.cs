using GovAuthSDK.DTO;
using GovAuthSDK.Enums;
using GovAuthSDK.Exceptions;
using GovAuthSDK.Extensions;
using GovAuthSDK.Helpers;
using GovAuthSDK.Models;
using Microsoft.EntityFrameworkCore;

namespace GovAuthSDK
{
    public sealed class GovAuthService
    {
        private AccessLevel _accessLevel = AccessLevel.Low;

        public GovAuthService()
        {
            using var context = new GovAuthContext();
            context.Database.EnsureCreatedAsync();
            context.Database.CanConnectAsync();
        }

        public async Task<TokenDto> TokenAuth(string token)
        {
            using var context = new GovAuthContext();

            if (JwtHelper.ValidateToken(token) == null)
                throw new InvalidTokenException(token);

            var dbToken = await context.Tokens.AsNoTracking().FirstOrDefaultAsync(x => x.AuthToken == token);

            if (dbToken == null)
                throw new InvalidTokenException(token);

            _accessLevel = dbToken.AccessLevel;

            return dbToken.ToDto();
        }

        public async Task<UserDto> LoginAuth(string login, string password)
        {
            using var context = new GovAuthContext();

            var dbUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Login == login);

            if (dbUser == null)
                throw new UserNotFoundException(login);

            if (!HashHelper.Match(dbUser.PasswordHashed, password))
                throw new WrongPasswordException(null);

            _accessLevel = dbUser.AccessLevel;

            return dbUser.ToDto();
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async Task<IEnumerable<UserDto>> AllUsers()
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovAuthContext();

            var dbUsers = await context.Users.AsNoTracking().ToListAsync();

            return dbUsers.Select(x => x.ToDto());
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async void AddUser(string login, string password, AccessLevel accessLevel)
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovAuthContext();

            var isExistUserWithSameLogin = await context.Users.AsNoTracking().AnyAsync(x => x.Login == login);

            if (isExistUserWithSameLogin)
                throw new ExistUserWithSameLoginException(login);

            var newUser = new User(login, HashHelper.HashPassword(password), accessLevel);

            await context.Users.AddAsync(newUser);

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async void DeleteUser(Guid id)
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovAuthContext();

            var user = await context.Users.FindAsync(id);

            if (user == null)
                throw new UserNotFoundException(id);

            context.Remove(user);

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async Task<IEnumerable<TokenDto>> AllTokens()
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovAuthContext();

            var dbTokens = await context.Tokens.AsNoTracking().ToListAsync();

            return dbTokens.Select(x => x.ToDto());
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async Task<string> AddToken(string owner, DateTime cancellation)
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovAuthContext();

            var newToken = new Token(owner, cancellation);

            await context.Tokens.AddAsync(newToken);

            await context.SaveChangesAsync();

            return newToken.AuthToken;
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async void DeleteToken(Guid id)
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovAuthContext();

            var token = await context.Tokens.FindAsync(id);

            if (token == null)
                throw new TokenNotFoundException(id);

            context.Remove(token);

            await context.SaveChangesAsync();
        }
    }
}