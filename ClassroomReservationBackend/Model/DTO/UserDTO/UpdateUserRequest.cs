using System.ComponentModel.DataAnnotations;

namespace ClassroomReservationBackend.Model.DTO.UserDTO;

public class UpdateUserRequest
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(20)]
    public string? Role { get; set; } // Admin, User

    public bool? IsActive { get; set; }
}