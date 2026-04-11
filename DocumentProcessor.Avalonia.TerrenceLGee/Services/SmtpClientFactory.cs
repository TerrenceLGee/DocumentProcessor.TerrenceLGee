using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using MailKit.Net.Smtp;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Services;

public class SmtpClientFactory : ISmtpClientFactory
{
    public ISmtpClient Create()
    {
        return new SmtpClient();
    }
}
