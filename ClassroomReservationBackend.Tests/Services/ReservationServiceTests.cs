using ClassroomReservationBackend.Data;
using ClassroomReservationBackend.Model.DTO.ReservationDTO;
using ClassroomReservationBackend.Model.Entity;
using ClassroomReservationBackend.Service.ReservationService;
using Microsoft.EntityFrameworkCore;

namespace ClassroomReservationBackend.Tests;

[TestFixture]
public class ReservationServiceTests
{
    private AppDbContext _context;
    private ReservationService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new ReservationService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task CreateAsync_WhenEndTimeBeforeStartTime_ThrowsArgumentException()
    {
        var request = new CreateReservationRequest
        {
            ClassroomId = Guid.NewGuid(),
            Title = "Test",
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(1)
        };

        Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(Guid.NewGuid(), request));
    }

    [Test]
    public async Task CreateAsync_WhenStartTimeInPast_ThrowsArgumentException()
    {
        var request = new CreateReservationRequest
        {
            ClassroomId = Guid.NewGuid(),
            Title = "Test",
            StartTime = DateTime.UtcNow.AddHours(-2),
            EndTime = DateTime.UtcNow.AddHours(-1)
        };

        Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(Guid.NewGuid(), request));
    }

    
    [Test]
    public async Task CreateAsync_WhenClassroomNotFound_ThrowsKeyNotFoundException()
    {
        var request = new CreateReservationRequest
        {
            ClassroomId = Guid.NewGuid(),
            Title = "Test",
            StartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(9),
            EndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(11)
        };
        Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.CreateAsync(Guid.NewGuid(), request));
    }

    [Test]
    public async Task CreateAsync_WhenClassroomNotActive_ThrowsInvalidOperationException()
    {
        var classroomId = Guid.NewGuid();
        var classroom = new Classroom
        {
            Id = classroomId,
            Name = "Test Room",
            RoomNumber = "101",
            Location = "Building A",
            Capacity = 30,
            ClassroomType = "Lecture",
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Classrooms.Add(classroom);
        await _context.SaveChangesAsync();

        var request = new CreateReservationRequest
        {
            ClassroomId = classroomId,
            Title = "Test",
            StartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(9),
            EndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(11)
        };
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.CreateAsync(Guid.NewGuid(), request));
    }

    [Test]
    public async Task DeleteAsync_WhenUserIsNotOwner_ThrowsUnauthorizedAccessException()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            ClassroomId = Guid.NewGuid(),
            UserId = userId,
            Title = "Test",
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.DeleteAsync(reservation.Id, otherUserId, false));
    }

    [Test]
    public async Task UpdateStatusAsync_WhenInvalidStatus_ThrowsArgumentException()
    {
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            ClassroomId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = "Test",
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        var request = new ReservationStatusRequest { Status = "InvalidStatus" };

        Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateStatusAsync(reservation.Id, Guid.NewGuid(), request));
    }

    [Test]
    public async Task GetAllAsync_WhenNoReservations_ReturnsEmptyList()
    {
        var filter = new ReservationFilterRequest();
        var result = await _service.GetAllAsync(filter);
        Assert.That(result, Is.Empty);
    }
}