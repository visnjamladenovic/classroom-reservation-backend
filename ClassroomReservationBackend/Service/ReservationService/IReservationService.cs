using ClassroomReservationBackend.Model.DTO.ReservationDTO;
using ClassroomReservationBackend.Model.DTO.ReservationDTO;

namespace ClassroomReservationBackend.Service.ReservationService;

public interface IReservationService
{
    Task<IEnumerable<ReservationResponse>> GetAllAsync(ReservationFilterRequest filter);
    Task<IEnumerable<ReservationResponse>> GetMyReservationsAsync(Guid userId, ReservationFilterRequest filter);
    Task<ReservationResponse> GetByIdAsync(Guid id);
    Task<ReservationResponse> CreateAsync(Guid userId, CreateReservationRequest request);
    Task<ReservationResponse> UpdateAsync(Guid id, Guid userId, bool isAdmin, UpdateReservationRequest request);
    Task<ReservationResponse> UpdateStatusAsync(Guid id, Guid adminId, ReservationStatusRequest request);
    Task DeleteAsync(Guid id, Guid userId, bool isAdmin);
}