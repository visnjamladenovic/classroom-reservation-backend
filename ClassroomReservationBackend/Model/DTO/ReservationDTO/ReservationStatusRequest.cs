namespace ClassroomReservationBackend.Model.DTO.ReservationDTO;

using System.ComponentModel.DataAnnotations;


public class ReservationStatusRequest
{
    [Required]
    public string Status { get; set; } = string.Empty; // Approved / Rejected / Cancelled
}
