using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Models.EmailModels;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;

public interface IEmailService
{
    Task<Result> SendEmailAsync(EmailData emailData);
}
