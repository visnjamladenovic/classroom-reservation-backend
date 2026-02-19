namespace ClassroomReservationBackend.Models.DTOs;

public class AuthResponse
{
    public string AccessToken {get;set;} = string.Empty;
    public string RefreshToken {get;set;} = string.Empty;
    
    public DateTime Expires {get;set;}
    public string Role {get;set;} = string.Empty;
    public string Email {get;set;} = string.Empty;
}