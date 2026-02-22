using ClassroomReservationBackend.Controller;
using ClassroomReservationBackend.Model.DTO.UserDTO;
using ClassroomReservationBackend.Service.UserService;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ClassroomReservationBackend.Tests;

[TestFixture]
public class UserControllerTests
{
    private Mock<IUserService> _mockService;
    private UserController _controller;

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<IUserService>();
        _controller = new UserController(_mockService.Object);
    }

    [Test]
    public async Task GetAll_ReturnsOkWithUsers()
    {
        _mockService.Setup(s => s.GetAllAsync(null))
            .ReturnsAsync(new List<UserResponse>
            {
                new UserResponse { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" }
            });
        var result = await _controller.GetAll(null);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetById_WhenExists_ReturnsOk()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.GetByIdAsync(id))
            .ReturnsAsync(new UserResponse { Id = id, FirstName = "John" });
        var result = await _controller.GetById(id);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Update_ReturnsOkWithUpdatedUser()
    {
        var id = Guid.NewGuid();
        var request = new UpdateUserRequest { FirstName = "Updated" };
        _mockService.Setup(s => s.UpdateAsync(id, request))
            .ReturnsAsync(new UserResponse { Id = id, FirstName = "Updated" });
        var result = await _controller.Update(id, request);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Delete_ReturnsNoContent()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);
        var result = await _controller.Delete(id);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}