namespace ClassroomReservationBackend.Model.DTO.ClassroomDTO;

public class ClassroomFilterRequest
{
    public string? Search { get; set; }          
    public string? ClassroomType { get; set; }
    public int? MinCapacity { get; set; }
    public int? MaxCapacity { get; set; }
    public bool? HasProjector { get; set; }
    public bool? HasWhiteboard { get; set; }
    public bool? HasComputers { get; set; }
    public bool? IsActive { get; set; }
    
    public DateTime? AvailableFrom { get; set; }
    public DateTime? AvailableTo { get; set; }
}


