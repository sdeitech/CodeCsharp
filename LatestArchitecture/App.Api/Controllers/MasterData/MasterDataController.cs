using App.Application.Interfaces.Services.MasterData;
using App.Application.Service.MasterTimeZones;
using App.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.MasterData
{
    [ServiceFilter(typeof(DateTimeConversionFilterService))]
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController(IMasterDataService masterService) : ControllerBase
    {
        private readonly IMasterDataService _masterService = masterService;

        /// <summary>
        /// Get master data by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("get-master-data/{key}")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMasterDataAsync(string key)
        {
            var response = await _masterService.GetMasterDataAsync(key);
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                //StatusCodes.Status401Unauthorized => Unauthorized(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, response)
            };
        }

        /// <summary>
        /// Get Master States by Country ID
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet("get-states/{countryId}")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMasterStateAsync(int countryId)
        {
            var response = await _masterService.GetMasterStateAsync(countryId);
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                //StatusCodes.Status401Unauthorized => Unauthorized(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, response)
            };
        }
    }
}
