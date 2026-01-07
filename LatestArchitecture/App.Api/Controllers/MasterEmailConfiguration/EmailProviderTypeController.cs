using App.Application.Interfaces.Services.EmailFactory;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.MasterAdmin
{
    /// <summary>
    /// Controller for managing email provider type operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmailProviderTypeController : ControllerBase
    {
        private readonly IEmailProviderTypeService _emailProviderTypeService;

        public EmailProviderTypeController(IEmailProviderTypeService emailProviderTypeService)
        {
            _emailProviderTypeService = emailProviderTypeService ?? throw new ArgumentNullException(nameof(emailProviderTypeService));
        }

        /// <summary>
        /// Gets all active email provider types
        /// </summary>
        /// <returns>A JsonModel containing the list of email provider types</returns>
        [HttpGet("get-all")]
        [ProducesResponseType(typeof(Common.Models.JsonModel), 200)]
        [ProducesResponseType(typeof(Common.Models.JsonModel), 500)]
        public async Task<IActionResult> GetAllActiveProviderTypes()
        {
            try
            {
                var result = await _emailProviderTypeService.GetAllActiveProviderTypesAsync();

                if (result.StatusCode == 200)
                {
                    return Ok(result);
                }
                else if (result.StatusCode == 404)
                {
                    return NotFound(result);
                }
                else
                {
                    return StatusCode(result.StatusCode, result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Common.Models.JsonModel
                {
                    Data = null,
                    Message = "An unexpected error occurred while retrieving email provider types.",
                    StatusCode = 500
                });
            }
        }
    }
}
