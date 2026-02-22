using System.Security.Claims;
using ClassroomReservationBackend.Model.DTO.ReservationDTO;
using ClassroomReservationBackend.Service.ReservationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomReservationBackend.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    /// <summary>
    /// Get all reservations (Admin only), with optional filters.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] ReservationFilterRequest filter)
    {
        var reservations = await _reservationService.GetAllAsync(filter);
        return Ok(reservations);
    }

    /// <summary>
    /// Get current user's reservations, with optional filters.
    /// </summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMy([FromQuery] ReservationFilterRequest filter)
    {
        var userId = GetUserId();
        var reservations = await _reservationService.GetMyReservationsAsync(userId, filter);
        return Ok(reservations);
    }

    /// <summary>
    /// Get a single reservation by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);

        // Regular users can only view their own reservations
        if (!IsAdmin() && reservation.UserId != GetUserId())
            return Forbid();

        return Ok(reservation);
    }

    /// <summary>
    /// Create a new reservation.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequest request)
    {
        var userId = GetUserId();
        var reservation = await _reservationService.CreateAsync(userId, request);
        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
    }

    /// <summary>
    /// Update an existing reservation (owner or Admin).
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReservationRequest request)
    {
        var userId = GetUserId();
        var isAdmin = IsAdmin();
        var reservation = await _reservationService.UpdateAsync(id, userId, isAdmin, request);
        return Ok(reservation);
    }

    /// <summary>
    /// Approve, reject, or cancel a reservation (Admin only).
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] ReservationStatusRequest request)
    {
        var adminId = GetUserId();
        var reservation = await _reservationService.UpdateStatusAsync(id, adminId, request);
        return Ok(reservation);
    }

    /// <summary>
    /// Cancel own reservation (sets status to Cancelled).
    /// </summary>
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var userId = GetUserId();
        var existing = await _reservationService.GetByIdAsync(id);

        if (existing.UserId != userId && !IsAdmin())
            return Forbid();

        var result = await _reservationService.UpdateStatusAsync(id, userId, new ReservationStatusRequest { Status = "Cancelled" });
        return Ok(result);
    }

    /// <summary>
    /// Delete a reservation (Admin or owner of Pending reservation).
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        var isAdmin = IsAdmin();
        await _reservationService.DeleteAsync(id, userId, isAdmin);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? throw new UnauthorizedAccessException("User ID not found in token.");
        return Guid.Parse(value);
    }

    private bool IsAdmin() =>
        User.IsInRole("Admin");
}