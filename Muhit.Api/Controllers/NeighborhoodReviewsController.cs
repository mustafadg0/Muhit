using Microsoft.AspNetCore.Mvc;
using Muhit.Application.DTOs.NeighborhoodReview.Request;
using Muhit.Application.Interfaces;

namespace Muhit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NeighborhoodReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public NeighborhoodReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateNeighborhoodReviewRequest request)
    {
        var result = await _reviewService.CreateAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}