using ClassroomReservationBackend.Data;
using ClassroomReservationBackend.Model.DTO.ClassroomDTO;
using ClassroomReservationBackend.Model.Entity;
using ClassroomReservationBackend.Service.ClassroomService;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace ClassroomReservationBackend.Tests;

[TestFixture]
public class MockClassroomServiceTests
{
    private Mock<AppDbContext> _mockContext;
    
    private ClassroomService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _mockContext = new Mock<AppDbContext>(options);
        _service = new ClassroomService(_mockContext.Object);
    }

    [Test]
    public async Task GetByIdAsync_WhenClassroomExists_ReturnsClassroom()
    {
        var classroomId = Guid.NewGuid();
        var classrooms = new List<Classroom>
        {
            new Classroom
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
            }
        };
        var mockDbSet = classrooms.AsQueryable().BuildMockDbSet();
        mockDbSet.Setup(m => m.FindAsync(classroomId))
            .ReturnsAsync(classrooms[0]);
        _mockContext.Setup(c => c.Classrooms).Returns(mockDbSet.Object);
        var result = await _service.GetByIdAsync(classroomId);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Test Room"));
    }

    [Test]
    public async Task GetByIdAsync_WhenClassroomDoesNotExist_ThrowsKeyNotFoundException()
    {
        var mockDbSet = new List<Classroom>().AsQueryable().BuildMockDbSet();
        mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
            .ReturnsAsync((Classroom?)null);
        _mockContext.Setup(c => c.Classrooms).Returns(mockDbSet.Object);
        Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.GetByIdAsync(Guid.NewGuid()));
    }

    [Test]
    public async Task DeleteAsync_WhenClassroomDoesNotExist_ThrowsKeyNotFoundException()
    {
        var mockDbSet = new List<Classroom>().AsQueryable().BuildMockDbSet();
        mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
            .ReturnsAsync((Classroom?)null);
        _mockContext.Setup(c => c.Classrooms).Returns(mockDbSet.Object);
        Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.DeleteAsync(Guid.NewGuid()));
    }
}