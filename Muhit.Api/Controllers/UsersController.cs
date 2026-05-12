using Microsoft.AspNetCore.Mvc;
using Muhit.Application.DTOs.User.Request;
using Muhit.Application.Interfaces;

namespace Muhit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAllAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _userService.GetByIdAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
    {
        var result = await _userService.UpdateAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _userService.DeleteAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost("profile-image")]
    public async Task<IActionResult> UploadProfileImage(IFormFile file)
    {
        var userId = 1; // TODO: JWT'den alınacak

        var result = await _userService.UploadProfileImageAsync(userId, file);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}