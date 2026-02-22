using ClassroomReservationBackend.Data;
using ClassroomReservationBackend.Model.DTO.UserDTO;
using ClassroomReservationBackend.Model.Entity;
using ClassroomReservationBackend.Service.UserService;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace ClassroomReservationBackend.Tests;

[TestFixture]
public class UserServiceTests
{
    private AppDbContext _context;
    private UserService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _service = new UserService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
    {
        var user = new User
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
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var result = await _service.GetByIdAsync(user.Id);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo("john@test.com"));
    }

    [Test]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ThrowsKeyNotFoundException()
    {
        Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.GetByIdAsync(Guid.NewGuid()));
    }

    [Test]
    public async Task UpdateAsync_WhenInvalidRole_ThrowsArgumentException()
    {
        var user = new User
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
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var request = new UpdateUserRequest { Role = "SuperAdmin" };
        Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateAsync(user.Id, request));
    }

    [Test]
    public async Task ChangePasswordAsync_WhenWrongCurrentPassword_ThrowsUnauthorizedAccessException()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            Role = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "wrongpassword",
            NewPassword = "newpassword123"
        };
        Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.ChangePasswordAsync(user.Id, request));
    }

    [Test]
    public async Task DeleteAsync_WhenUserExists_DeletesSuccessfully()
    {
        var user = new User
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
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await _service.DeleteAsync(user.Id);
        var deleted = await _context.Users.FindAsync(user.Id);
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_WhenSearchByName_ReturnsMatchingUsers()
    {
        _context.Users.AddRange(
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
        );
        await _context.SaveChangesAsync();
        var result = await _service.GetAllAsync("John");
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().FirstName, Is.EqualTo("John"));
    }
}