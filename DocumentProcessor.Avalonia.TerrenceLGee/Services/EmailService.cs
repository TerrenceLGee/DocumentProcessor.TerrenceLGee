using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Helpers;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Models.EmailModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Services;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _configuration;
    private readonly ISmtpClientFactory _smtpClientFactory;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailConfiguration> configuration, 
        ISmtpClientFactory smtpClientFactory, 
        ILogger<EmailService> logger)
    {
        _configuration = configuration.Value;
        _smtpClientFactory = smtpClientFactory;
        _logger = logger;
    }
    public async Task<Result> SendEmailAsync(EmailData emailData)
    {
        var errorMessage = string.Empty;
        try
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_configuration.SenderName, _configuration.SenderEmail));
            email.To.Add(new MailboxAddress(emailData.ReceiverName, emailData.ReceiverEmail));

            email.Subject = emailData.Subject;

            var body = EmailHelper.GetFormattedEmailText(emailData, _configuration.SenderName);

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };

            if (emailData.Attachments.Count > 0)
            {
                foreach (var attachment in emailData.Attachments)
                {
                    if (!string.IsNullOrEmpty(attachment.FilePath))
                    {
                        builder.Attachments.Add(attachment.FilePath);
                    }
                }
            }

            email.Body = builder.ToMessageBody();

            using (var smtp = _smtpClientFactory.Create())
            {
                await smtp.ConnectAsync(_configuration.Host, _configuration.Port);

                await smtp.AuthenticateAsync(_configuration.SenderEmail, _configuration.Password);

                await smtp.SendAsync(email);

                await smtp.DisconnectAsync(true);

                return Result.Ok();
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"{LogMessageHelper.GetMessageForLogging(nameof(EmailService), nameof(SendEmailAsync))}" +
                $"There was an unexpected error sending email to: {emailData.ReceiverEmail}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return Result.Fail($"There was an unexpected error sending email to: {emailData.ReceiverEmail}");
        }
    }
}
