using ClassroomReservationBackend.Model.DTO.ClassroomDTO;
using ClassroomReservationBackend.Service.ClassroomService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomReservationBackend.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClassroomController : ControllerBase
{
    private readonly IClassroomService _classroomService;

    public ClassroomController(IClassroomService classroomService)
    {
        _classroomService = classroomService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ClassroomFilterRequest filter)
    {
        var classrooms = await _classroomService.GetAllAsync(filter);
        return Ok(classrooms);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var classroom = await _classroomService.GetByIdAsync(id);
        return Ok(classroom);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateClassroomRequest request)
    {
        var classroom = await _classroomService.CreateAsync(request);
        return Ok(classroom);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClassroomRequest request)
    {
        var classroom = await _classroomService.UpdateAsync(id, request);
        return Ok(classroom);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _classroomService.DeleteAsync(id);
        return NoContent();
    }
}