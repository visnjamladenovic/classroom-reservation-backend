using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomReservationBackend.Models;

[Table("reservations")]
public class Reservation
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("classroom_id")]
    public Guid ClassroomId { get; set; }

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [Column("start_time")]
    public DateTime StartTime { get; set; }

    [Required]
    [Column("end_time")]
    public DateTime EndTime { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "Pending";

    [MaxLength(50)]
    [Column("purpose")]
    public string Purpose { get; set; } = "Lecture";

    [Column("attendee_count")]
    public int? AttendeeCount { get; set; }

    [Column("approved_by")]
    public Guid? ApprovedBy { get; set; }

    [Column("approved_at")]
    public DateTime? ApprovedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("ClassroomId")]
    public Classroom Classroom { get; set; } = null!;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [ForeignKey("ApprovedBy")]
    public User? ApprovedByUser { get; set; }
}
