using GovAuthSDK;
using GovAuthSDK.Enums;
using GovTransportSDK.DTO;
using GovTransportSDK.Exceptions;
using GovTransportSDK.Extensions;
using GovTransportSDK.Helpers;
using GovTransportSDK.Models;
using Microsoft.EntityFrameworkCore;

namespace GovTransportSDK
{
    public sealed class GovTransportService
    {
        private AccessLevel _accessLevel = AccessLevel.Low;

        public GovTransportService()
        {
            using var context = new GovTransportContext();
            context.Database.EnsureCreatedAsync();
            context.Owners.FirstOrDefault();
        }

        public async Task<string> Auth(string token)
        {
            var authService = new GovAuthService();
            var authResult = await authService.TokenAuth(token);

            _accessLevel = authResult.AccessLevel;

            return authResult.Description;
        }

        public async Task<string> Auth(string login, string password)
        {
            var authService = new GovAuthService();
            var authResult = await authService.LoginAuth(login, password);

            _accessLevel = authResult.AccessLevel;

            return authResult.Login;
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

            await context.SaveChangesAsync();
        }
    }
}
