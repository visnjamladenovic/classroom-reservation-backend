using ClassroomReservationBackend.Data;
using ClassroomReservationBackend.Model.DTO.ReservationDTO;
using ClassroomReservationBackend.Model.Entity;
using ClassroomReservationBackend.Service.ReservationService;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace ClassroomReservationBackend.Tests;

[TestFixture]
public class MockReservationServiceTests
{
    private Mock<AppDbContext> _mockContext;
    private ReservationService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _mockContext = new Mock<AppDbContext>(options);
        _service = new ReservationService(_mockContext.Object);
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
        var mockDbSet = new List<Classroom>().AsQueryable().BuildMockDbSet();
        mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((Classroom?)null);
        _mockContext.Setup(c => c.Classrooms).Returns(mockDbSet.Object);
        var request = new CreateReservationRequest
        {
            ClassroomId = Guid.NewGuid(),
            Title = "Test",
            StartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(10),
            EndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(12)
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
        var mockDbSet = new List<Classroom> { classroom }.AsQueryable().BuildMockDbSet();
        mockDbSet.Setup(m => m.FindAsync(classroomId)).ReturnsAsync(classroom);
        _mockContext.Setup(c => c.Classrooms).Returns(mockDbSet.Object);
        var request = new CreateReservationRequest
        {
            ClassroomId = classroomId,
            Title = "Test",
            StartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(10),
            EndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(12)
        };
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.CreateAsync(Guid.NewGuid(), request));
    }
    [Test]
    public async Task DeleteAsync_WhenUserIsNotOwner_ThrowsUnauthorizedAccessException()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var reservationId = Guid.NewGuid();
        var reservation = new Reservation
        {
            Id = reservationId,
            ClassroomId = Guid.NewGuid(),
            UserId = userId,
            Title = "Test",
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var mockDbSet = new List<Reservation> { reservation }.AsQueryable().BuildMockDbSet();
        mockDbSet.Setup(m => m.FindAsync(reservationId)).ReturnsAsync(reservation);
        _mockContext.Setup(c => c.Reservations).Returns(mockDbSet.Object);
        Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.DeleteAsync(reservationId, otherUserId, false));
    }

    [Test]
    public async Task UpdateStatusAsync_WhenInvalidStatus_ThrowsArgumentException()
    {
        var reservationId = Guid.NewGuid();
        var reservation = new Reservation
        {
            Id = reservationId,
            ClassroomId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = "Test",
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var mockDbSet = new List<Reservation> { reservation }.AsQueryable().BuildMockDbSet();
        mockDbSet.Setup(m => m.FindAsync(reservationId)).ReturnsAsync(reservation);
        _mockContext.Setup(c => c.Reservations).Returns(mockDbSet.Object);
        var request = new ReservationStatusRequest { Status = "InvalidStatus" };
        Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateStatusAsync(reservationId, Guid.NewGuid(), request));
    }

    [Test]
    public async Task GetAllAsync_WhenNoReservations_ReturnsEmptyList()
    {
        var mockDbSet = new List<Reservation>().AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Reservations).Returns(mockDbSet.Object);
        var filter = new ReservationFilterRequest();
        var result = await _service.GetAllAsync(filter);
        Assert.That(result, Is.Empty);
    }
    [Test]
    public async Task CreateAsync_WhenStartTimeBeforeWorkingHours_ThrowsArgumentException()
    {
        var request = new CreateReservationRequest
        {
            ClassroomId = Guid.NewGuid(),
            Title = "Test",
            StartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(6),
            EndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(9)
        };
        Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(Guid.NewGuid(), request));
    }

    [Test]
    public async Task CreateAsync_WhenEndTimeAfterWorkingHours_ThrowsArgumentException()
    {
        var request = new CreateReservationRequest
        {
            ClassroomId = Guid.NewGuid(),
            Title = "Test",
            StartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(18),
            EndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(21)
        };
        Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(Guid.NewGuid(), request));
    }
    [Test]
    public async Task CreateAsync_WhenAttendeeCountExceedsCapacity_ThrowsArgumentException()
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
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var mockDbSet = new List<Classroom> { classroom }.AsQueryable().BuildMockDbSet();
        mockDbSet.Setup(m => m.FindAsync(classroomId)).ReturnsAsync(classroom);
        _mockContext.Setup(c => c.Classrooms).Returns(mockDbSet.Object);
        var mockResDbSet = new List<Reservation>().AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Reservations).Returns(mockResDbSet.Object);
        var request = new CreateReservationRequest
        {
            ClassroomId = classroomId,
            Title = "Test",
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            AttendeeCount = 50
        };
        Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(Guid.NewGuid(), request));
    }
}