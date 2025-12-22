using App.SuperAdmin.Application.Dto;
using App.SuperAdmin.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace App.SuperAdmin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterController(IMasterService masterService) : BaseController
    {
        private readonly IMasterService _masterService = masterService;

        [HttpPost]
        [Route("CreateMasterDataAsync")]
        public async Task<IActionResult> CreateMasterDataAsync([FromBody] MasterDto masterDto)
        {
            return Ok(await _masterService.CreateMasterDataAsync(masterDto));
        }
    }
}
