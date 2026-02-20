using ClassroomReservationBackend.Models.DTOs;

namespace ClassroomReservationBackend.Services;

public interface IClassroomService
{
    Task<IEnumerable<ClassroomResponse>> GetAllAsync();
    Task<ClassroomResponse> GetByIdAsync(Guid id);
    Task<ClassroomResponse> CreateAsync(CreateClassroomRequest request);
    Task<ClassroomResponse> UpdateAsync(Guid id, UpdateClassroomRequest request);
    Task DeleteAsync(Guid id);
}