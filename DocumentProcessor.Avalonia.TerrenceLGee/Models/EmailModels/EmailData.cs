namespace DocumentProcessor.Avalonia.TerrenceLGee.Models.EmailModels;

public class EmailData
{
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? FilePath { get; set; }
}
