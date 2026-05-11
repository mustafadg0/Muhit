using Microsoft.AspNetCore.Mvc;
using Muhit.Application.DTOs.Neighborhood.Request;
using Muhit.Application.Interfaces;

namespace Muhit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NeighborhoodAiAnalysisController : ControllerBase
{
    private readonly IAiNeighborhoodAnalysisService _aiNeighborhoodAnalysisService;

    public NeighborhoodAiAnalysisController(
        IAiNeighborhoodAnalysisService aiNeighborhoodAnalysisService)
    {
        _aiNeighborhoodAnalysisService = aiNeighborhoodAnalysisService;
    }

    [HttpPost("Generate")]
    public async Task<IActionResult> Generate(
        [FromBody] GenerateNeighborhoodAiAnalysisRequest request)
    {
        var result = await _aiNeighborhoodAnalysisService.GenerateAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}