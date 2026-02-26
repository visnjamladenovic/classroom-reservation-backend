using ClassroomReservationBackend.Data;
using ClassroomReservationBackend.Model.DTO.UserDTO;
using ClassroomReservationBackend.Model.Entity;
using ClassroomReservationBackend.Service.UserService;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace ClassroomReservationBackend.Tests;

[TestFixture]
public class MockUserServiceTests
{
    private Mock<AppDbContext> _mockContext;
    private UserService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _mockContext = new Mock<AppDbContext>(options);
        _service = new UserService(_mockContext.Object);
    }

    [Test]
    public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
    {
        var userId = Guid.NewGuid();
        var users = new List<User>
        {
            new User
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                PasswordHash = "hash",
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        var mockDbSet = users.AsQueryable().BuildMockDbSet();
        mockDbSet.Setup(m => m.FindAsync(userId)).ReturnsAsync(users[0]);
        _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);
        var result = await _service.GetByIdAsync(userId);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo("john@test.com"));
    }

    [Test]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ThrowsKeyNotFoundException()
    {
        var mockDbSet = new List<User>().AsQueryable().BuildMockDbSet();
        mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((User?)null);
        _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);
        Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.GetByIdAsync(Guid.NewGuid()));
    }

    [Test]
    public async Task UpdateAsync_WhenInvalidRole_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var users = new List<User>
        {
            new User
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                PasswordHash = "hash",
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        var mockDbSet = users.AsQueryable().BuildMockDbSet();
        mockDbSet.Setup(m => m.FindAsync(userId)).ReturnsAsync(users[0]);
        _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);
        var request = new UpdateUserRequest { Role = "SuperAdmin" };
        Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateAsync(userId, request));
    }

    [Test]
    public async Task ChangePasswordAsync_WhenWrongCurrentPassword_ThrowsUnauthorizedAccessException()
    {
        var userId = Guid.NewGuid();
        var users = new List<User>
        {
            new User
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        var mockDbSet = users.AsQueryable().BuildMockDbSet();
        mockDbSet.Setup(m => m.FindAsync(userId)).ReturnsAsync(users[0]);
        _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "wrongpassword",
            NewPassword = "newpassword123"
        };
        Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.ChangePasswordAsync(userId, request));
    }

    [Test]
    public async Task DeleteAsync_WhenUserDoesNotExist_ThrowsKeyNotFoundException()
    {
        var mockDbSet = new List<User>().AsQueryable().BuildMockDbSet();
        mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((User?)null);
        _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);
        Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.DeleteAsync(Guid.NewGuid()));
    }

    [Test]
    public async Task GetAllAsync_WhenSearchByName_ReturnsMatchingUsers()
    {
        var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                PasswordHash = "hash",
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@test.com",
                PasswordHash = "hash",
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        var mockDbSet = users.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);
        var result = await _service.GetAllAsync("John");
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().FirstName, Is.EqualTo("John"));
    }
}