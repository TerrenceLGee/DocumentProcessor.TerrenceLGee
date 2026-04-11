using MailKit.Net.Smtp;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;

public interface ISmtpClientFactory
{
    ISmtpClient Create();
}
