using ClassroomReservationBackend.Controller;
using ClassroomReservationBackend.Model.DTO;
using ClassroomReservationBackend.Service.AuthService;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ClassroomReservationBackend.Tests;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAuthService> _mockService;
    private AuthController _controller;

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<IAuthService>();
        _controller = new AuthController(_mockService.Object);
    }

    [Test]
    public async Task Register_ReturnsOk()
    {
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Password = "password123"
        };
        _mockService.Setup(s => s.RegisterAsync(request))
            .ReturnsAsync(new AuthResponse { Email = "john@test.com" });
        var result = await _controller.Register(request);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Login_ReturnsOk()
    {
        var request = new LoginRequest
        {
            Email = "john@test.com",
            Password = "password123"
        };
        _mockService.Setup(s => s.LoginAsync(request))
            .ReturnsAsync(new AuthResponse { Email = "john@test.com" });
        var result = await _controller.Login(request);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Refresh_ReturnsOk()
    {
        var token = "somerefreshtoken";
        _mockService.Setup(s => s.RefreshTokenAsync(token))
            .ReturnsAsync(new AuthResponse { Email = "john@test.com" });
        var result = await _controller.Refresh(token);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Logout_ReturnsNoContent()
    {
        var token = "somerefreshtoken";
        _mockService.Setup(s => s.RevokeTokenAsync(token)).Returns(Task.CompletedTask);
        var result = await _controller.Logout(token);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}