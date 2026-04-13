using DocumentProcessor.Avalonia.TerrenceLGee.Models.EmailModels;

namespace DocumentProcessor.UnitTests.TerrenceLGee.Resources;

public static class EmailResources
{
    public static EmailData GetEmailData()
    {
        return new()
        {
            ReceiverName = "Homer J. Simpson",
            ReceiverEmail = "HSimpson@example.com",
            Subject = "Meeting with Principal Skinner today",
            Body = "Homer, please remember that we have a meeting with Principal Skinner today about Bart."
        };
    }

    public static EmailConfiguration GetConfiguration()
    {
        return new()
        {
            SenderEmail = "MSimpson@example.com",
            SenderName = "Marjorie B. Simpson",
            Host = "smtp.test.com",
            Password = "Te$tP@$$w0rd",
            Port = 123
        };
    }
}
