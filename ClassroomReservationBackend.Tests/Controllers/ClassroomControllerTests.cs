using ClassroomReservationBackend.Controller;
using ClassroomReservationBackend.Model.DTO.ClassroomDTO;
using ClassroomReservationBackend.Service.ClassroomService;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ClassroomReservationBackend.Tests;

[TestFixture]
public class ClassroomControllerTests
{
    private Mock<IClassroomService> _mockService;
    private ClassroomController _controller;

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<IClassroomService>();
        _controller = new ClassroomController(_mockService.Object);
    }

    
    [Test]
    public async Task GetAll_ReturnsOkWithClassrooms()
    {
        var filter = new ClassroomFilterRequest();
        _mockService.Setup(s => s.GetAllAsync(filter))
            .ReturnsAsync(new List<ClassroomResponse>
            {
                new ClassroomResponse { Id = Guid.NewGuid(), Name = "Room 1" }
            });
        var result = await _controller.GetAll(filter);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetById_WhenExists_ReturnsOk()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.GetByIdAsync(id))
            .ReturnsAsync(new ClassroomResponse { Id = id, Name = "Room 1" });
        var result = await _controller.GetById(id);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Create_ReturnsOkWithCreatedClassroom()
    {
        var request = new CreateClassroomRequest
            { Name = "New Room", RoomNumber = "101", Location = "A", Capacity = 20, ClassroomType = "Lecture" };
        _mockService.Setup(s => s.CreateAsync(request))
            .ReturnsAsync(new ClassroomResponse { Id = Guid.NewGuid(), Name = "New Room" });
        var result = await _controller.Create(request);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Update_ReturnsOkWithUpdatedClassroom()
    {
        var id = Guid.NewGuid();
        var request = new UpdateClassroomRequest { Name = "Updated Room" };
        _mockService.Setup(s => s.UpdateAsync(id, request))
            .ReturnsAsync(new ClassroomResponse { Id = id, Name = "Updated Room" });
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