using Microsoft.AspNetCore.Mvc;
using Muhit.Application.Interfaces;

namespace Muhit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NeighborhoodsController : ControllerBase
    {
        private readonly INeighborhoodService _neighborhoodService;
        private readonly INeighborhoodDetailService _neighborhoodDetailService;
        private readonly INeighborhoodAiAnalysisAppService _aiAnalysisAppService;

        public NeighborhoodsController(
            INeighborhoodService neighborhoodService,
            INeighborhoodDetailService neighborhoodDetailService,
            INeighborhoodAiAnalysisAppService aiAnalysisAppService)
        {
            _neighborhoodService = neighborhoodService;
            _neighborhoodDetailService = neighborhoodDetailService;
            _aiAnalysisAppService = aiAnalysisAppService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _neighborhoodService.GetAllAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _neighborhoodService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("GetByDistrictId/{districtId}")]
        public async Task<IActionResult> GetByDistrictId(int districtId)
        {
            var result = await _neighborhoodService.GetByDistrictIdAsync(districtId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var result = await _neighborhoodService.SearchAsync(keyword);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("Detail")]
        public async Task<IActionResult> GetDetail(
            [FromQuery] string city,
            [FromQuery] string district,
            [FromQuery] string neighborhood)
        {
            var result = await _neighborhoodDetailService.GetDetailAsync(
                city,
                district,
                neighborhood);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("{neighborhoodId}/GenerateAiAnalysis")]
        public async Task<IActionResult> GenerateAiAnalysis(int neighborhoodId)
        {
            var result = await _aiAnalysisAppService.GenerateAsync(neighborhoodId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}