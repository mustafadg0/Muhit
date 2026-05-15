using Microsoft.AspNetCore.Mvc;
using Muhit.Application.Interfaces;

namespace Muhit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DistrictsController : ControllerBase
    {
        private readonly IDistrictService _districtService;

        public DistrictsController(IDistrictService districtService)
        {
            _districtService = districtService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByCityId([FromQuery] int cityId)
        {
            var result = await _districtService.GetByCityIdAsync(cityId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
