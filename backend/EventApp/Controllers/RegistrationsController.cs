using System.Security.Claims;
using EventApp.DTOs.Common;
using EventApp.DTOs.Queries;
using EventApp.DTOs.Registrations;
using EventApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class RegistrationsController(IRegistrationService registrationService) : ControllerBase
{
    [HttpPost("events/{eventId:guid}")]
    [ProducesResponseType(typeof(RegistrationResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(Guid eventId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await registrationService.RegisterAsync(userId, eventId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpDelete("events/{eventId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(Guid eventId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await registrationService.CancelAsync(userId, eventId, cancellationToken);
        return NoContent();
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(PagedResponse<RegistrationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyRegistrations([FromQuery] PaginationQuery query, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await registrationService.GetMyRegistrationsAsync(userId, query, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PagedResponse<RegistrationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] RegistrationQuery query, CancellationToken cancellationToken)
    {
        var result = await registrationService.GetAllAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{registrationId:guid}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RegistrationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStatus(Guid registrationId, [FromBody] RegistrationStatusUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await registrationService.UpdateStatusAsync(registrationId, request.Status, cancellationToken);
        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

        if (userId is null || !Guid.TryParse(userId, out var parsed))
        {
            throw new UnauthorizedAccessException("Invalid token payload.");
        }

        return parsed;
    }
}
