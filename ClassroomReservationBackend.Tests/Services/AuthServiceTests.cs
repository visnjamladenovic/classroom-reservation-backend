using ClassroomReservationBackend.Data;
using ClassroomReservationBackend.Model.DTO;
using ClassroomReservationBackend.Model.Entity;
using ClassroomReservationBackend.Service.AuthService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ClassroomReservationBackend.Tests;

[TestFixture]
public class AuthServiceTests
{
    private AppDbContext _context;
    private AuthService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "JwtSettings:SecretKey", "SuperSecretKeyThatIsAtLeast32CharsLong!" },
                { "JwtSettings:Issuer", "TestIssuer" },
                { "JwtSettings:Audience", "TestAudience" },
                { "JwtSettings:AccessTokenExpirationMinutes", "30" },
                { "JwtSettings:RefreshTokenExpirationDays", "7" }
            })
            .Build();
        _service = new AuthService(_context, config);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsInvalidOperationException()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            Role = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var request = new RegisterRequest
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "john@test.com",
            Password = "password123"
        };
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.RegisterAsync(request));
    }

    [Test]
    public async Task LoginAsync_WhenUserNotFound_ThrowsUnauthorizedAccessException()
    {
        var request = new LoginRequest
        {
            Email = "notfound@test.com",
            Password = "password"
        };
        Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.LoginAsync(request));
    }

    [Test]
    public async Task LoginAsync_WhenWrongPassword_ThrowsUnauthorizedAccessException()
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
        var request = new LoginRequest
        {
            Email = "john@test.com",
            Password = "wrongpassword"
        };
        Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.LoginAsync(request));
    }

    [Test]
    public async Task LoginAsync_WhenAccountDeactivated_ThrowsUnauthorizedAccessException()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            Role = "User",
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var request = new LoginRequest
        {
            Email = "john@test.com",
            Password = "password"
        };
        Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.LoginAsync(request));
    }
}