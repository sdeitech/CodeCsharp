using App.Application.Interfaces.Services.MasterTimeZones;
using App.Application.Service.MasterTimeZones;
using App.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.MasterTimeZones
{
    [ServiceFilter(typeof(DateTimeConversionFilterService))]
    [Route("api/[controller]")]
    [ApiController]
    public class MasterTimeZonesController : BaseController
    {
        private readonly IMasterTimeZonesService _masterTimeZonesService;
        public MasterTimeZonesController(IMasterTimeZonesService masterTimeZonesService)
        {
            _masterTimeZonesService = masterTimeZonesService;
        }
        /// <summary>
        /// Get all active time zones list.
        /// </summary>
        /// <returns>Returns list of time zones in a JsonModel</returns>
        [HttpGet("getAll")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _masterTimeZonesService.GetAllAsync();

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
    }
}
