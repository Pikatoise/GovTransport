using GovAuthSDK;
using GovAuthSDK.DTO;
using GovAuthSDK.Enums;
using GovTransportSDK.DTO;
using GovTransportSDK.Exceptions;
using GovTransportSDK.Extensions;
using GovTransportSDK.Helpers;
using GovTransportSDK.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GovTransportSDK
{
    public sealed class GovTransportService
    {
        private readonly GovAuthService _authService;
        private readonly ILogger<GovTransportService> _logger;
        private AccessLevel _accessLevel = AccessLevel.Low;
        private string? _identity;

        public GovTransportService(GovAuthService authService, ILogger<GovTransportService> logger)
        {
            using var context = new GovTransportContext();
            context.Database.EnsureCreatedAsync();
            context.Owners.FirstOrDefault();

            _authService = authService;
            _logger = logger;
        }

        public async Task<TokenDto> Auth(string token)
        {
            var authResult = await _authService.TokenAuth(token);

            _accessLevel = authResult.AccessLevel;
            _identity = authResult.TokenValue;

            return authResult;
        }

        public async Task<UserDto> Auth(string login, string password)
        {
            var authResult = await _authService.LoginAuth(login, password);

            _accessLevel = authResult.AccessLevel;
            _identity = authResult.Login;

            return authResult;
        }

        #region Ownership

        /// <summary>
        /// Low access level
        /// </summary>
        public async Task<VinInfoDto?> InfoByVin(string vin)
        {
            using var context = new GovTransportContext();

            var transportDb = await context.Transports.FirstOrDefaultAsync(x => x.VIN == vin);

            if (transportDb == null)
                return null;

            var historiesDb = await context.OwnerHistories
                .AsNoTracking()
                .Include(x => x.Ownership)
                .Where(x => x.TransportId == transportDb.Id)
                .ToListAsync();

            LogMethodExecution("InfoByVin");

            return new VinInfoDto()
            {
                Transport = transportDb.ToDto(),
                History = historiesDb.Select(x => x.ToMinimizedDto())
            };
        }

        /// <summary>
        /// Medium access level
        /// </summary>
        public async Task<IEnumerable<Ownership>> AllOwners()
        {
            if (_accessLevel == AccessLevel.Low)
                throw new NoAccessException(AccessLevel.Medium.ToString());

            using var context = new GovTransportContext();

            var ownersDb = await context.Owners.AsNoTracking().ToListAsync();

            LogMethodExecution("AllOwners");

            return ownersDb;
        }

        /// <summary>
        /// Medium access level
        /// </summary>
        public async Task<Ownership> OwnerById(Guid id)
        {
            if (_accessLevel == AccessLevel.Low)
                throw new NoAccessException(AccessLevel.Medium.ToString());

            using var context = new GovTransportContext();

            var ownerDb = await context.Owners.FindAsync(id);

            if (ownerDb == null)
                throw new OwnerNotFoundException(id);

            LogMethodExecution("OwnerById");

            return ownerDb;
        }

        /// <summary>
        /// Medium access level
        /// </summary>
        public async Task<Ownership?> OwnerByPassport(string passport)
        {
            if (_accessLevel == AccessLevel.Low)
                throw new NoAccessException(AccessLevel.Medium.ToString());

            using var context = new GovTransportContext();

            var ownerDb = await context.Owners.SingleOrDefaultAsync(x => EF.Functions.Like(x.Passport, $"{passport}"));

            LogMethodExecution("OwnerByPassport");

            return ownerDb;
        }

        /// <summary>
        /// Medium access level
        /// </summary>
        public async Task<IEnumerable<Ownership>> FindOwnersByPassport(string passport)
        {
            if (_accessLevel == AccessLevel.Low)
                throw new NoAccessException(AccessLevel.Medium.ToString());

            using var context = new GovTransportContext();

            var ownersDb = await context.Owners
                .AsNoTracking()
                .Where(x => EF.Functions.Like(x.Passport, $"%{passport}%"))
                .ToListAsync();

            LogMethodExecution("FindOwnersByPassport");

            return ownersDb;
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async Task<Guid> AddOwnership(AddOwnershipDto dto)
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovTransportContext();

            var ownerWithSamePassport = await context.Owners
                .AsNoTracking()
                .SingleOrDefaultAsync(x => EF.Functions.Like(x.Passport, $"{dto.Passport}"));

            if (ownerWithSamePassport != null)
                throw new OwnerWithSamePassportExistsException(dto.Passport);

            Ownership newOwnership = new Ownership(dto.FullName, dto.RegistrationAddress, dto.Passport, dto.Osago, dto.IsLegal);

            await context.Owners.AddAsync(newOwnership);

            await context.SaveChangesAsync();

            LogMethodExecution("AddOwnership");

            return newOwnership.Id;
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async Task UpdateOwnership(Ownership changedOwner)
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovTransportContext();

            var ownerWithSameId = await context.Owners.FindAsync(changedOwner.Id);

            if (ownerWithSameId == null)
                throw new OwnerNotFoundException(changedOwner.Id);

            ownerWithSameId.FullName = changedOwner.FullName;
            ownerWithSameId.RegistrationAddress = changedOwner.RegistrationAddress;
            ownerWithSameId.Passport = changedOwner.Passport;
            ownerWithSameId.Osago = changedOwner.Osago;
            ownerWithSameId.IsLegal = changedOwner.IsLegal;

            context.Owners.Update(ownerWithSameId);

            LogMethodExecution("UpdateOwnership");

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Medium access level
        /// </summary>
        public async Task<List<OwnerHistoryDetailedDto>> OwnershipTransportsHistory(Guid ownerId)
        {
            if (_accessLevel == AccessLevel.Low)
                throw new NoAccessException(AccessLevel.Medium.ToString());

            using var context = new GovTransportContext();

            var owner = await context.Owners.FindAsync(ownerId);

            if (owner == null)
                throw new OwnerNotFoundException(ownerId);

            var ownerTransportHistories = await context.OwnerHistories
                .AsNoTracking()
                .Include(x => x.Transport)
                .Where(x => x.OwnershipId == ownerId)
                .ToListAsync();

            LogMethodExecution("OwnershipTransportsHistory");

            return ownerTransportHistories.Select(x => x.ToDetailedDto()).ToList();
        }

        #endregion

        #region Transport

        /// <summary>
        /// Medium access level
        /// </summary>
        public async Task<IEnumerable<Transport>> AllTransports()
        {
            if (_accessLevel == AccessLevel.Low)
                throw new NoAccessException(AccessLevel.Medium.ToString());

            using var context = new GovTransportContext();

            var transportsDb = await context.Transports.AsNoTracking().ToListAsync();

            LogMethodExecution("AllTransports");

            return transportsDb;
        }

        /// <summary>
        /// Medium access level
        /// </summary>
        public async Task<Transport> TransportById(Guid id)
        {
            if (_accessLevel == AccessLevel.Low)
                throw new NoAccessException(AccessLevel.Medium.ToString());

            using var context = new GovTransportContext();

            var transportDb = await context.Transports.FindAsync(id);

            if (transportDb == null)
                throw new TransportNotFoundException(id);

            LogMethodExecution("TransportById");

            return transportDb;
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async Task<Guid> AddTransport(AddTransportDto dto)
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovTransportContext();

            var transportWithSameVin = await context.Transports
                .AsNoTracking()
                .SingleOrDefaultAsync(x => EF.Functions.Like(x.VIN, $"{dto.VIN}"));

            if (transportWithSameVin != null)
                throw new TransportWithSameVinExistsException(dto.VIN);

            Transport newTransport = new Transport(dto.VIN, dto.Model, dto.ReleaseYear, dto.Color, dto.GovNumber, dto.Status, dto.BodyType);

            await context.Transports.AddAsync(newTransport);

            await context.SaveChangesAsync();

            LogMethodExecution("AddTransport");

            return newTransport.Id;
        }

        /// <summary>
        /// Medium access level
        /// </summary>
        public async Task<Transport?> FindTransportByVIN(string vin)
        {
            if (_accessLevel == AccessLevel.Low)
                throw new NoAccessException(AccessLevel.Medium.ToString());

            using var context = new GovTransportContext();

            var transportDb = await context.Transports
                .AsNoTracking()
                .FirstOrDefaultAsync(x => EF.Functions.Like(x.VIN, $"{vin}"));

            LogMethodExecution("FindTransportByVIN");

            return transportDb;
        }

        /// <summary>
        /// Medium access level
        /// </summary>
        public async Task<IEnumerable<Transport>> FindTransportsByVIN(string vin)
        {
            if (_accessLevel == AccessLevel.Low)
                throw new NoAccessException(AccessLevel.Medium.ToString());

            using var context = new GovTransportContext();

            var transportsDb = await context.Transports
                .AsNoTracking()
                .Where(x => EF.Functions.Like(x.VIN, $"%{vin}%"))
                .ToListAsync();

            LogMethodExecution("FindTransportsByVIN");

            return transportsDb;
        }

        /// <summary>
        /// Medium access level
        /// </summary>
        public async Task<IEnumerable<Transport>> FindTransportsByGovNumber(string govNumber)
        {
            if (_accessLevel == AccessLevel.Low)
                throw new NoAccessException(AccessLevel.Medium.ToString());

            using var context = new GovTransportContext();

            var transportsDb = await context.Transports
                .AsNoTracking()
                .Where(x => EF.Functions.Like(x.GovNumber, $"%{govNumber}%"))
                .ToListAsync();

            LogMethodExecution("FindTransportsByGovNumber");

            return transportsDb;
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async Task UpdateTransport(Transport changedTransport)
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovTransportContext();

            var transportWithSameID = await context.Transports.FindAsync(changedTransport.Id);

            if (transportWithSameID == null)
                throw new TransportNotFoundException(changedTransport.Id);

            transportWithSameID.VIN = changedTransport.VIN;
            transportWithSameID.Model = changedTransport.Model;
            transportWithSameID.ReleaseYear = changedTransport.ReleaseYear;
            transportWithSameID.Color = changedTransport.Color;
            transportWithSameID.GovNumber = changedTransport.GovNumber;
            transportWithSameID.Status = changedTransport.Status;
            transportWithSameID.BodyType = changedTransport.BodyType;

            context.Transports.Update(transportWithSameID);

            LogMethodExecution("UpdateTransport");

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Medium access level
        /// </summary>
        public async Task<IEnumerable<OwnerHistoryDetailedDto>> TransportOwnersHistory(Guid transportId)
        {
            if (_accessLevel == AccessLevel.Low)
                throw new NoAccessException(AccessLevel.Medium.ToString());

            using var context = new GovTransportContext();

            var transport = await context.Transports.FindAsync(transportId);

            if (transport == null)
                throw new TransportNotFoundException(transportId);

            var transportOwnersHistories = await context.OwnerHistories
                .AsNoTracking()
                .Where(x => x.TransportId == transportId)
                .Include(x => x.Transport)
                .Include(x => x.Ownership)
                .ToListAsync();

            LogMethodExecution("TransportOwnersHistory");

            return transportOwnersHistories.Select(x => x.ToDetailedDto());
        }

        /// <summary>
        /// Medium access level
        /// </summary>
        public async Task<OwnerHistory?> LastOwnerByTransportId(Guid transportId)
        {
            if (_accessLevel == AccessLevel.Low)
                throw new NoAccessException(AccessLevel.Medium.ToString());

            using var context = new GovTransportContext();

            var transport = await context.Transports.FindAsync(transportId);

            if (transport == null)
                throw new TransportNotFoundException(transportId);

            var lastOwner = await context.OwnerHistories
                .AsNoTracking()
                .Include(x => x.Ownership)
                .OrderBy(x => x.Start)
                .LastOrDefaultAsync(x => x.TransportId == transportId);

            LogMethodExecution("LastOwnerByTransportId");

            return lastOwner;
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async Task<string> UpdateTransportGovNumber(Guid transportId, string regionCode)
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovTransportContext();

            var transport = await context.Transports.FindAsync(transportId);

            if (transport == null)
                throw new TransportNotFoundException(transportId);

            var newGovNumber = await GenerateUniqueGovNumber(regionCode);

            transport.GovNumber = newGovNumber;

            context.Transports.Update(transport);

            await context.SaveChangesAsync();

            LogMethodExecution("UpdateTransportGovNumber");

            return newGovNumber;
        }

        /// <summary>
        /// High access level
        /// </summary>
        public async Task<string> GenerateUniqueGovNumber(string regionCode)
        {
            using var context = new GovTransportContext();

            string newGovNumber = "";

            while (true)
            {
                newGovNumber = GovNumberHelper.GenerateGovNumber(regionCode);

                bool isUnique = await context.Transports.AnyAsync(x => EF.Functions.Like(x.GovNumber, $"{newGovNumber}"));

                if (!isUnique)
                    break;
            }

            LogMethodExecution("GenerateUniqueGovNumber");

            return newGovNumber;
        }

        #endregion

        /// <summary>
        /// Required High level access
        /// </summary>
        public async Task TransportOwnerRegistration(Guid ownerId, Guid transportId)
        {
            if (_accessLevel != AccessLevel.High)
                throw new NoAccessException(AccessLevel.High.ToString());

            using var context = new GovTransportContext();

            var owner = await context.Owners.FindAsync(ownerId);
            if (owner == null)
                throw new OwnerNotFoundException(ownerId);

            var transport = await context.Transports.FindAsync(transportId);
            if (transport == null)
                throw new TransportNotFoundException(transportId);

            var lastOwnerHistory = await context.OwnerHistories
                .AsNoTracking()
                .OrderBy(x => x.Start)
                .LastOrDefaultAsync(x => x.TransportId == transportId);

            if (lastOwnerHistory != null && lastOwnerHistory.End == null)
            {
                lastOwnerHistory.End = DateTime.UtcNow;

                context.OwnerHistories.Update(lastOwnerHistory);
            }

            var newHistory = new OwnerHistory(transportId, ownerId, DateTime.UtcNow, null);

            await context.OwnerHistories.AddAsync(newHistory);

            LogMethodExecution("TransportOwnerRegistration");

            await context.SaveChangesAsync();
        }

        private void LogMethodExecution(string methodName) =>
            _logger.LogInformation($"\nMethod {methodName} executed by '{_identity}' with access level '{_accessLevel}'\n");
    }
}
