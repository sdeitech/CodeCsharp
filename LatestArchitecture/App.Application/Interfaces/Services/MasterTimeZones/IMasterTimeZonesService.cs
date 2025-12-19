using App.Common.Models;

namespace App.Application.Interfaces.Services.MasterTimeZones
{
    public interface IMasterTimeZonesService
    {
        Task<JsonModel> GetAllAsync();
        DateTime UtcNow();
        DateTimeOffset UtcNowOffset();

        DateTime FromUtc(DateTime utc, string ianaTimeZoneId);
        DateTimeOffset FromUtcOffset(DateTime utc, string ianaTimeZoneId);

        DateTime ToUtc(DateTime local, string ianaTimeZoneId, bool throwOnInvalid = false);
        TimeZoneInfo GetTimeZoneInfo(string ianaTimeZoneId);
    }

}
