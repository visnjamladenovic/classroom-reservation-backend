namespace ClassroomReservationBackend.Model.DTO.ClassroomDTO;

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