using Microsoft.AspNetCore.Mvc;
using Muhit.Application.Interfaces;

namespace Muhit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NeighborhoodHomeController : ControllerBase
{
    private readonly INeighborhoodHomeService _neighborhoodHomeService;

    public NeighborhoodHomeController(INeighborhoodHomeService neighborhoodHomeService)
    {
        _neighborhoodHomeService = neighborhoodHomeService;
    }

    [HttpGet("GetDetail/{neighborhoodId}")]
    public async Task<IActionResult> GetDetail(int neighborhoodId)
    {
        var result = await _neighborhoodHomeService.GetDetailAsync(neighborhoodId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}