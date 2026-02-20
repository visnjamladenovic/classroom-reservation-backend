namespace ClassroomReservationBackend.Models.DTOs;

public class CreateClassroomRequest
{
    public string Name { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string ClassroomType { get; set; } = "Lecture";
    public bool HasProjector { get; set; }
    public bool HasWhiteboard { get; set; }
    public bool HasComputers { get; set; }
    public string? Description { get; set; }
}

public class UpdateClassroomRequest
{
    public string? Name { get; set; }
    public string? RoomNumber { get; set; }
    public string? Location { get; set; }
    public int? Capacity { get; set; }
    public string? ClassroomType { get; set; }
    public bool? HasProjector { get; set; }
    public bool? HasWhiteboard { get; set; }
    public bool? HasComputers { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

public class ClassroomResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string ClassroomType { get; set; } = string.Empty;
    public bool HasProjector { get; set; }
    public bool HasWhiteboard { get; set; }
    public bool HasComputers { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}