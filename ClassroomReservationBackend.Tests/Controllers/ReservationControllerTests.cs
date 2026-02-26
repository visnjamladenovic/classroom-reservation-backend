using System.Security.Claims;
using ClassroomReservationBackend.Controller;
using ClassroomReservationBackend.Model.DTO.ReservationDTO;
using ClassroomReservationBackend.Service.ReservationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace ClassroomReservationBackend.Tests;

[TestFixture]
public class ReservationControllerTests
{
    private Mock<IReservationService> _mockService;
    private ReservationController _controller;

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<IReservationService>();
        _controller = new ReservationController(_mockService.Object);
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "User")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Test]
    public async Task GetAll_ReturnsOk()
    {
        var filter = new ReservationFilterRequest();
        _mockService.Setup(s => s.GetAllAsync(filter))
            .ReturnsAsync(new List<ReservationResponse>
            {
                new ReservationResponse { Id = Guid.NewGuid(), Title = "Test" }
            });
        var result = await _controller.GetAll(filter);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Update_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var request = new UpdateReservationRequest { Title = "Updated" };
        _mockService.Setup(s => s.UpdateAsync(id, It.IsAny<Guid>(), It.IsAny<bool>(), request))
            .ReturnsAsync(new ReservationResponse { Id = id, Title = "Updated" });
        var result = await _controller.Update(id, request);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Delete_ReturnsNoContent()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(id, It.IsAny<Guid>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);
        var result = await _controller.Delete(id);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}