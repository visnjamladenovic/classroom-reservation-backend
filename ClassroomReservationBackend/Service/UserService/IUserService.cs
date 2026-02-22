using ClassroomReservationBackend.Model.DTO.UserDTO;

namespace ClassroomReservationBackend.Service.UserService;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync(string? search);
    Task<UserResponse> GetByIdAsync(Guid id);
    Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<UserResponse> UpdateSelfAsync(Guid id, UpdateUserRequest request);
    Task ChangePasswordAsync(Guid id, ChangePasswordRequest request);
    Task DeleteAsync(Guid id);
}