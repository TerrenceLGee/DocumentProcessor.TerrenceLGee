namespace DocumentProcessor.Avalonia.TerrenceLGee.DTOs;

public class RetrievedContactDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleInitial { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string TelephoneNumber { get; set; } = string.Empty;
}
