using ClassroomReservationBackend.Data;
using ClassroomReservationBackend.Model.DTO.ClassroomDTO;
using ClassroomReservationBackend.Service.ClassroomService;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace ClassroomReservationBackend.Tests;

[TestFixture]
public class ClassroomServiceTests
{
    private AppDbContext _context;
    private ClassroomService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new ClassroomService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetByIdAsync_WhenClassroomExists_ReturnsClassroom()
    {
        var classroom = new Classroom
        {
            Id = Guid.NewGuid(),
            Name = "Test Room",
            RoomNumber = "101",
            Location = "Building A",
            Capacity = 30,
            ClassroomType = "Lecture",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Classrooms.Add(classroom);
        await _context.SaveChangesAsync();

        var result = await _service.GetByIdAsync(classroom.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Test Room"));
    }

    [Test]
    public async Task GetByIdAsync_WhenClassroomDoesNotExist_ThrowsKeyNotFoundException()
    {
        Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.GetByIdAsync(Guid.NewGuid()));
    }

    [Test]
    public async Task CreateAsync_CreatesClassroomSuccessfully()
    {
        var request = new CreateClassroomRequest
        {
            Name = "New Room",
            RoomNumber = "202",
            Location = "Building B",
            Capacity = 20,
            ClassroomType = "Lab"
        };

        var result = await _service.CreateAsync(request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("New Room"));
        Assert.That(result.RoomNumber, Is.EqualTo("202"));
    }

    [Test]
    public async Task UpdateAsync_WhenClassroomExists_UpdatesSuccessfully()
    {
        var classroom = new Classroom
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            RoomNumber = "101",
            Location = "Building A",
            Capacity = 30,
            ClassroomType = "Lecture",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Classrooms.Add(classroom);
        await _context.SaveChangesAsync();

        var request = new UpdateClassroomRequest { Name = "New Name" };
        var result = await _service.UpdateAsync(classroom.Id, request);

        Assert.That(result.Name, Is.EqualTo("New Name"));
    }

    [Test]
    public async Task DeleteAsync_WhenClassroomExists_DeletesSuccessfully()
    {
        var classroom = new Classroom
        {
            Id = Guid.NewGuid(),
            Name = "To Delete",
            RoomNumber = "303",
            Location = "Building C",
            Capacity = 15,
            ClassroomType = "Lecture",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Classrooms.Add(classroom);
        await _context.SaveChangesAsync();

        await _service.DeleteAsync(classroom.Id);

        var deleted = await _context.Classrooms.FindAsync(classroom.Id);
        Assert.That(deleted, Is.Null);
        Assert.False(_context.Classrooms.Any());
    }

    [Test]
    public async Task DeleteAsync_WhenClassroomDoesNotExist_ThrowsKeyNotFoundException()
    {
        Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.DeleteAsync(Guid.NewGuid()));
    }
}