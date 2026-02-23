using ClassroomReservationBackend.Model.DTO.ClassroomDTO;

namespace ClassroomReservationBackend.Service.ClassroomService;

public interface IClassroomService
{
    Task<IEnumerable<ClassroomResponse>> GetAllAsync(ClassroomFilterRequest? filter = null);
    Task<ClassroomResponse> GetByIdAsync(Guid id);
    Task<ClassroomResponse> CreateAsync(CreateClassroomRequest request);
    Task<ClassroomResponse> UpdateAsync(Guid id, UpdateClassroomRequest request);
    Task DeleteAsync(Guid id);
}