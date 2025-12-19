using App.Application.Interfaces.Repositories.MasterTimeZone;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Application.Interfaces.Services.MasterTimeZones;
using App.Common.Constant;
using App.Common.Models;
using AutoMapper;
using System.Collections.Concurrent;
using System.Net;
using TimeZoneConverter;

namespace App.Application.Service.MasterTimeZones
{
    public class MasterTimeZonesService : IMasterTimeZonesService
    {
        private readonly IMasterTimeZonesRepository _repository;
        private static readonly ConcurrentDictionary<string, TimeZoneInfo> _cache = new();


        public MasterTimeZonesService(
           IMasterTimeZonesRepository repository,
           ICurrentUserClaimService currentUserClaimService,
           IMapper mapper)
        {
            _repository = repository;
            
        }
     

        public DateTime FromUtc(DateTime utc, string ianaTimeZoneId)
        {
            if (utc == DateTime.MinValue || utc == DateTime.MaxValue) return utc;
            if (utc.Kind != DateTimeKind.Utc) utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);

            var tz = GetTimeZoneInfo(ianaTimeZoneId);
            var local = TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
            // Return Unspecified so it isn't mistaken for system local
            return DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
        }

        public DateTimeOffset FromUtcOffset(DateTime utc, string ianaTimeZoneId)
        {
            if (utc.Kind != DateTimeKind.Utc) utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            var tz = GetTimeZoneInfo(ianaTimeZoneId);
            var offset = tz.GetUtcOffset(utc);
            return new DateTimeOffset(utc).ToOffset(offset);
        }

       

        public TimeZoneInfo GetTimeZoneInfo(string ianaTimeZoneId)
        {
            if (string.IsNullOrWhiteSpace(ianaTimeZoneId)) return TimeZoneInfo.Utc;

            return _cache.GetOrAdd(ianaTimeZoneId, id =>
            {
                try
                {
                    return TZConvert.GetTimeZoneInfo(id);
                }
                catch
                {
                    // fallback to UTC if unknown
                    return TimeZoneInfo.Utc;
                }
            });
        }

        public DateTime ToUtc(DateTime local, string ianaTimeZoneId, bool throwOnInvalid = false)
        {
            if (local == DateTime.MinValue || local == DateTime.MaxValue) return local;

            // Treat incoming as wall-clock time in specified tz
            local = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);

            var tz = GetTimeZoneInfo(ianaTimeZoneId);

            if (tz.IsInvalidTime(local)) // <-- correct
            {
                if (throwOnInvalid)
                    throw new ArgumentException($"Local time {local} is invalid in timezone {ianaTimeZoneId} (DST gap).");
                local = local.AddMinutes(1);
            }

            var utc = TimeZoneInfo.ConvertTimeToUtc(local, tz);
            return DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        }


        public DateTime UtcNow() => DateTime.UtcNow;

        public DateTimeOffset UtcNowOffset() => DateTimeOffset.UtcNow;


        public async Task<JsonModel> GetAllAsync()
        {
            var settings = await _repository.GetAllAsync();

            if (settings == null || !settings.Any())
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = StatusMessage.NoSettingsFound,
                    Data = false
                };
            }

            return new JsonModel
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = StatusMessage.SettingsRetrieved,
                Data = settings
            };
        }
    }
}
