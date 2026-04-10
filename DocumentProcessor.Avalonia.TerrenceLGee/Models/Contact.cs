using System.ComponentModel.DataAnnotations;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Models;

public class Contact
{
    public int Id { get; set; }
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(2, ErrorMessage = "Middle initial cannot exceed 2 characters (The initial and a period).")]
    public string? MiddleInitial { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telephone number is required.")]
    public string TelephoneNumber { get; set; } = string.Empty;
}
