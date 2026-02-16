using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomReservationBackend.Models;

[Table("classrooms")]
public class Classroom
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("room_number")]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("location")]
    public string Location { get; set; } = string.Empty;

    [Required]
    [Column("capacity")]
    public int Capacity { get; set; }

    [Required]
    [MaxLength(30)]
    [Column("classroom_type")]
    public string ClassroomType { get; set; } = "Lecture";

    [Column("has_projector")]
    public bool HasProjector { get; set; } = false;

    [Column("has_whiteboard")]
    public bool HasWhiteboard { get; set; } = true;

    [Column("has_computers")]
    public bool HasComputers { get; set; } = false;

    [Column("description")]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
