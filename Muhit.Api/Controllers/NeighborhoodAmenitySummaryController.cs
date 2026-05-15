using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Muhit.Application.DTOs.Neighborhood.Request;
using Muhit.Application.Interfaces;
using Muhit.Persistence.Context;

namespace Muhit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NeighborhoodAmenitySummaryController : ControllerBase
{
    private readonly MuhitDbContext _context;
    private readonly INeighborhoodAmenityService _amenityService;

    public NeighborhoodAmenitySummaryController(MuhitDbContext context, INeighborhoodAmenityService amenityService)
    {
        _context = context;
        _amenityService = amenityService;
    }

    [HttpPost("summary")]
    public async Task<IActionResult> GetOrCreateSummary([FromBody] NeighborhoodAmenitySummaryRequest request)
    {
        // Mahalleyi il, ilçe, mahalle adý ile bul
        var neighborhood = await _context.Neighborhoods
            .Include(n => n.District)
                .ThenInclude(d => d.City)
            .FirstOrDefaultAsync(n =>
                n.Name == request.Neighborhood &&
                n.District.Name == request.District &&
                n.District.City.Name == request.City);

        if (neighborhood == null)
            return NotFound("Mahalle bulunamadý.");

        var result = await _amenityService.GetOrCreateSummaryAsync(neighborhood.Id);
        if (!result.Success)
            return BadRequest(result.Message);
        return Ok(result.Data);
    }
}
