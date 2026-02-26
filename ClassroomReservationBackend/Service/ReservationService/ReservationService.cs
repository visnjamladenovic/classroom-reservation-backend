using ClassroomReservationBackend.Data;
using ClassroomReservationBackend.Model.DTO.ReservationDTO;
using ClassroomReservationBackend.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace ClassroomReservationBackend.Service.ReservationService;

public class ReservationService : IReservationService
{
    private readonly AppDbContext _context;

    public ReservationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReservationResponse>> GetAllAsync(ReservationFilterRequest filter)
    {
        var query = _context.Reservations
            .Include(r => r.Classroom)
            .Include(r => r.User)
            .Include(r => r.ApprovedByUser)
            .AsQueryable();

        query = ApplyFilters(query, filter);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => MapToResponse(r))
            .ToListAsync();
    }

    public async Task<IEnumerable<ReservationResponse>> GetMyReservationsAsync(Guid userId,
        ReservationFilterRequest filter)
    {
        var query = _context.Reservations
            .Include(r => r.Classroom)
            .Include(r => r.User)
            .Include(r => r.ApprovedByUser)
            .Where(r => r.UserId == userId)
            .AsQueryable();

        query = ApplyFilters(query, filter);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => MapToResponse(r))
            .ToListAsync();
    }

    public async Task<ReservationResponse> GetByIdAsync(Guid id)
    {
        var reservation = await _context.Reservations
                              .Include(r => r.Classroom)
                              .Include(r => r.User)
                              .Include(r => r.ApprovedByUser)
                              .FirstOrDefaultAsync(r => r.Id == id)
                          ?? throw new KeyNotFoundException("Reservation not found.");

        return MapToResponse(reservation);
    }

    public async Task<ReservationResponse> CreateAsync(Guid userId, CreateReservationRequest request)
    {
        if (request.EndTime <= request.StartTime)
            throw new ArgumentException("End time must be after start time.");

        if (request.StartTime < DateTime.UtcNow)
            throw new ArgumentException("Cannot create a reservation in the past.");
        if (request.StartTime.Hour < 8 || request.StartTime.Hour >= 20)
            throw new ArgumentException("Reservations can only start between 08:00 and 20:00.");

        if (request.EndTime.Hour > 20 || (request.EndTime.Hour == 20 && request.EndTime.Minute > 0))
            throw new ArgumentException("Reservations must end by 20:00.");

        var classroom = await _context.Classrooms.FindAsync(request.ClassroomId)
                        ?? throw new KeyNotFoundException("Classroom not found.");

        if (!classroom.IsActive)
            throw new InvalidOperationException("Classroom is not available.");

        var hasConflict = await _context.Reservations.AnyAsync(r =>
            r.ClassroomId == request.ClassroomId &&
            r.Status != "Rejected" &&
            r.Status != "Cancelled" &&
            r.StartTime < request.EndTime &&
            r.EndTime > request.StartTime);

        if (hasConflict)
            throw new InvalidOperationException("The classroom is already booked for this time slot.");
        if (request.AttendeeCount.HasValue && request.AttendeeCount.Value > classroom.Capacity)
            throw new ArgumentException($"Attendee count ({request.AttendeeCount.Value}) exceeds classroom capacity ({classroom.Capacity}).");

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            ClassroomId = request.ClassroomId,
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Status = "Pending",
            Purpose = request.Purpose,
            AttendeeCount = request.AttendeeCount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(reservation.Id);
    }

    public async Task<ReservationResponse> UpdateAsync(Guid id, Guid userId, bool isAdmin,
        UpdateReservationRequest request)
    {
        var reservation = await _context.Reservations.FindAsync(id)
                          ?? throw new KeyNotFoundException("Reservation not found.");

        if (!isAdmin && reservation.UserId != userId)
            throw new UnauthorizedAccessException("You can only edit your own reservations.");

        if (!isAdmin && reservation.Status != "Pending")
            throw new InvalidOperationException("Only pending reservations can be edited.");

        var newStart = request.StartTime ?? reservation.StartTime;
        var newEnd = request.EndTime ?? reservation.EndTime;

        if (newEnd <= newStart)
            throw new ArgumentException("End time must be after start time.");

        var hasConflict = await _context.Reservations.AnyAsync(r =>
            r.Id != id &&
            r.ClassroomId == reservation.ClassroomId &&
            r.Status != "Rejected" &&
            r.Status != "Cancelled" &&
            r.StartTime < newEnd &&
            r.EndTime > newStart);

        if (hasConflict)
            throw new InvalidOperationException("The classroom is already booked for this time slot.");

        if (request.Title != null) reservation.Title = request.Title;
        if (request.Description != null) reservation.Description = request.Description;
        if (request.StartTime != null) reservation.StartTime = request.StartTime.Value;
        if (request.EndTime != null) reservation.EndTime = request.EndTime.Value;
        if (request.Purpose != null) reservation.Purpose = request.Purpose;
        if (request.AttendeeCount != null) reservation.AttendeeCount = request.AttendeeCount;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<ReservationResponse> UpdateStatusAsync(Guid id, Guid adminId, ReservationStatusRequest request)
    {
        var validStatuses = new[] { "Approved", "Rejected", "Cancelled" };
        if (!validStatuses.Contains(request.Status))
            throw new ArgumentException($"Invalid status. Valid values: {string.Join(", ", validStatuses)}");

        var reservation = await _context.Reservations.FindAsync(id)
                          ?? throw new KeyNotFoundException("Reservation not found.");

        if (reservation.Status != "Pending")
            throw new InvalidOperationException("Only pending reservations can be approved or rejected.");

        reservation.Status = request.Status;
        reservation.UpdatedAt = DateTime.UtcNow;

        if (request.Status == "Approved") {
            reservation.ApprovedBy = adminId;
            reservation.ApprovedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(Guid id, Guid userId, bool isAdmin)
    {
        var reservation = await _context.Reservations.FindAsync(id)
                          ?? throw new KeyNotFoundException("Reservation not found.");

        if (!isAdmin && reservation.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own reservations.");

        if (!isAdmin && reservation.Status == "Approved")
            throw new InvalidOperationException("Cannot delete an approved reservation. Please cancel it first.");

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
    }

    private static IQueryable<Reservation> ApplyFilters(IQueryable<Reservation> query, ReservationFilterRequest filter)
    {
        if (filter.ClassroomId.HasValue)
            query = query.Where(r => r.ClassroomId == filter.ClassroomId.Value);

        if (filter.UserId.HasValue)
            query = query.Where(r => r.UserId == filter.UserId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Status))
            query = query.Where(r => r.Status == filter.Status);

        if (!string.IsNullOrWhiteSpace(filter.Purpose))
            query = query.Where(r => r.Purpose == filter.Purpose);

        if (filter.From.HasValue)
            query = query.Where(r => r.StartTime >= filter.From.Value);

        if (filter.To.HasValue)
            query = query.Where(r => r.EndTime <= filter.To.Value);

        return query;
    }

    private static ReservationResponse MapToResponse(Reservation r) => new()
    {
        Id = r.Id,
        ClassroomId = r.ClassroomId,
        ClassroomName = r.Classroom?.Name ?? string.Empty,
        RoomNumber = r.Classroom?.RoomNumber ?? string.Empty,
        UserId = r.UserId,
        UserFullName = r.User != null ? $"{r.User.FirstName} {r.User.LastName}" : string.Empty,
        UserEmail = r.User?.Email ?? string.Empty,
        Title = r.Title,
        Description = r.Description,
        StartTime = r.StartTime,
        EndTime = r.EndTime,
        Status = r.Status,
        Purpose = r.Purpose,
        AttendeeCount = r.AttendeeCount,
        ApprovedBy = r.ApprovedBy,
        ApprovedByName = r.ApprovedByUser != null
            ? $"{r.ApprovedByUser.FirstName} {r.ApprovedByUser.LastName}"
            : null,
        ApprovedAt = r.ApprovedAt,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };
}