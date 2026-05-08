using EventApp.DTOs.Common;
using EventApp.DTOs.Events;
using EventApp.DTOs.Queries;
using EventApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EventsController(IEventService eventService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponse<EventResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] EventQuery query, CancellationToken cancellationToken)
    {
        var events = await eventService.GetAllAsync(query, cancellationToken);
        return Ok(events);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var eventResponse = await eventService.GetByIdAsync(id, cancellationToken);
        return Ok(eventResponse);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] EventRequest request, CancellationToken cancellationToken)
    {
        var created = await eventService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EventRequest request, CancellationToken cancellationToken)
    {
        var updated = await eventService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await eventService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
