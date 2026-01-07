using App.Application.Interfaces.Repositories.MasterTimeZone;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using App.Common.Utility;

namespace App.Application.Service.MasterTimeZones
{
    public class DateTimeConversionFilterService : IAsyncResultFilter
    {
        private readonly IMasterTimeZonesRepository _masterTimeZoneRepository;

        public DateTimeConversionFilterService(IMasterTimeZonesRepository masterTimeZoneRepository)
        {
            _masterTimeZoneRepository = masterTimeZoneRepository;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                // Get system timezone dynamically from DB
                var systemTimeZone = await _masterTimeZoneRepository.GetActiveTimeZoneAsync();
                var timeZoneInfo = TimeZoneConverter.TZConvert.GetTimeZoneInfo(systemTimeZone);
                if (objectResult.Value.GetType().Name == "JsonModel")
                {
                    dynamic json = objectResult.Value;
                    if (json.Data != null)
                    {
                        CommonMethods.ConvertDateTimesToTimeZone(json.Data, timeZoneInfo);
                    }
                }
                else
                {
                    CommonMethods.ConvertDateTimesToTimeZone(objectResult.Value, timeZoneInfo);
                }
            }

            await next();
        }
    }

}
