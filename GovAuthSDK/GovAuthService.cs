using GovAuthSDK.DTO;
using GovAuthSDK.Enums;
using GovAuthSDK.Exceptions;
using GovAuthSDK.Extensions;
using GovAuthSDK.Helpers;
using GovAuthSDK.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GovAuthSDK
{
    public sealed class GovAuthService
    {
        private readonly ILogger<GovAuthService> _logger;
        private AccessLevel _accessLevel = AccessLevel.Low;
        private string? _identity;

        public GovAuthService(ILogger<GovAuthService> logger)
        {
            using var context = new GovAuthContext();
            context.Database.EnsureCreatedAsync();
            context.Users.FirstOrDefault();

            _logger = logger;
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
            _identity = dbToken.AuthToken;

            LogMethodExecution("TokenAuth");

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
            _identity = dbUser.Login;

            LogMethodExecution("LoginAuth");

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

            LogMethodExecution("AllUsers");

            return dbUsers.Select(x => x.ToDto());
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async Task AddUser(string login, string password)
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovAuthContext();

            var isExistUserWithSameLogin = await context.Users.AsNoTracking().AnyAsync(x => x.Login == login);

            if (isExistUserWithSameLogin)
                throw new ExistUserWithSameLoginException(login);

            var newUser = new User(login, HashHelper.HashPassword(password));

            await context.Users.AddAsync(newUser);

            LogMethodExecution("AddUser");

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async Task DeleteUser(Guid id)
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovAuthContext();

            var user = await context.Users.FindAsync(id);

            if (user == null)
                throw new UserNotFoundException(id);

            context.Remove(user);

            LogMethodExecution("DeleteUser");

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

            LogMethodExecution("AllTokens");

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

            LogMethodExecution("AddToken");

            return newToken.AuthToken;
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async Task DeleteToken(string token)
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovAuthContext();

            var tokenDb = await context.Tokens.AsNoTracking().SingleOrDefaultAsync(x => EF.Functions.Like(x.AuthToken, token));

            if (tokenDb == null)
                throw new TokenNotFoundException(token);

            context.Remove(tokenDb);

            LogMethodExecution("DeleteToken");

            await context.SaveChangesAsync();
        }

        private void LogMethodExecution(string methodName) =>
            _logger.LogInformation($"\nMethod {methodName} executed by '{_identity}' with access level '{_accessLevel}'\n");
    }
}