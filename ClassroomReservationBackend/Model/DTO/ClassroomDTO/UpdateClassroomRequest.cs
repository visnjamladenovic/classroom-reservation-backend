namespace ClassroomReservationBackend.Model.DTO.ClassroomDTO;

public class UpdateClassroomRequest
{
    public string? Name{ get; set; }
    public string? RoomNumber{ get; set; }
    public string? Location{ get; set; }
    public int? Capacity{ get; set; }
    public string? ClassroomType{ get; set; }
    public bool? HasProjector{ get; set; }
    public bool? HasWhiteboard{ get; set; }
    public bool? HasComputers{ get; set; }
    public string? Description{ get; set; }
    public bool? IsActive{ get; set; }
}