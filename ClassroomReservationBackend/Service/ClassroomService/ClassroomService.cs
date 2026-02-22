using ClassroomReservationBackend.Data;
using ClassroomReservationBackend.Model.DTO.ClassroomDTO;
using Microsoft.EntityFrameworkCore;

namespace ClassroomReservationBackend.Service.ClassroomService;

public class ClassroomService : IClassroomService
{
    private readonly AppDbContext _context;

    public ClassroomService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClassroomResponse>> GetAllAsync()
    {
        return await _context.Classrooms
            .Select(c => MapToResponse(c))
            .ToListAsync();
    }

    public async Task<ClassroomResponse> GetByIdAsync(Guid id)
    {
        var classroom = await _context.Classrooms.FindAsync(id)
                        ?? throw new KeyNotFoundException("Classroom not found.");

        return MapToResponse(classroom);
    }

    public async Task<ClassroomResponse> CreateAsync(CreateClassroomRequest request)
    {
        var classroom = new Classroom
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            RoomNumber = request.RoomNumber,
            Location = request.Location,
            Capacity = request.Capacity,
            ClassroomType = request.ClassroomType,
            HasProjector = request.HasProjector,
            HasWhiteboard = request.HasWhiteboard,
            HasComputers = request.HasComputers,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Classrooms.Add(classroom);
        await _context.SaveChangesAsync();

        return MapToResponse(classroom);
    }

    public async Task<ClassroomResponse> UpdateAsync(Guid id, UpdateClassroomRequest request)
    {
        var classroom = await _context.Classrooms.FindAsync(id)
                        ?? throw new KeyNotFoundException("Classroom not found.");

        if (request.Name != null) classroom.Name = request.Name;
        if (request.RoomNumber != null) classroom.RoomNumber = request.RoomNumber;
        if (request.Location != null) classroom.Location = request.Location;
        if (request.Capacity != null) classroom.Capacity = request.Capacity.Value;
        if (request.ClassroomType != null) classroom.ClassroomType = request.ClassroomType;
        if (request.HasProjector != null) classroom.HasProjector = request.HasProjector.Value;
        if (request.HasWhiteboard != null) classroom.HasWhiteboard = request.HasWhiteboard.Value;
        if (request.HasComputers != null) classroom.HasComputers = request.HasComputers.Value;
        if (request.Description != null) classroom.Description = request.Description;
        if (request.IsActive != null) classroom.IsActive = request.IsActive.Value;
        classroom.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponse(classroom);
    }

    public async Task DeleteAsync(Guid id)
    {
        var classroom = await _context.Classrooms.FindAsync(id)
                        ?? throw new KeyNotFoundException("Classroom not found.");

        _context.Classrooms.Remove(classroom);
        await _context.SaveChangesAsync();
    }

    private static ClassroomResponse MapToResponse(Classroom c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        RoomNumber = c.RoomNumber,
        Location = c.Location,
        Capacity = c.Capacity,
        ClassroomType = c.ClassroomType,
        HasProjector = c.HasProjector,
        HasWhiteboard = c.HasWhiteboard,
        HasComputers = c.HasComputers,
        Description = c.Description,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}