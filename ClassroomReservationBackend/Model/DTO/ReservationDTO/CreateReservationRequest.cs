using System.ComponentModel.DataAnnotations;

namespace ClassroomReservationBackend.Model.DTO.ReservationDTO;

public class CreateReservationRequest
{
    [Required]
    public Guid ClassroomId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [MaxLength(50)]
    public string Purpose { get; set; } = "Lecture";

    public int? AttendeeCount { get; set; }
}