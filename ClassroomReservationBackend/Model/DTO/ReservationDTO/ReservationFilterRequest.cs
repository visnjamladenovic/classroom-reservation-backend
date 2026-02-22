namespace ClassroomReservationBackend.Model.DTO.ReservationDTO;

public class ReservationFilterRequest
{
    public Guid? ClassroomId { get; set; }
    public Guid? UserId { get; set; }
    public string? Status { get; set; }
    public string? Purpose { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}