using Microsoft.AspNetCore.Mvc;
using Muhit.Application.Interfaces;

namespace Muhit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _cityService.GetAllAsync();
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
