using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using DocumentProcessor.UnitTests.TerrenceLGee.Resources;
using DocumentProcessor.Avalonia.TerrenceLGee.Models.EmailModels;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Services;
using Microsoft.Extensions.Logging;

namespace DocumentProcessor.UnitTests.TerrenceLGee;

public class EmailServiceTests
{
    private readonly OptionsWrapper<EmailConfiguration> _configWrapper;
    private readonly Mock<ISmtpClientFactory> _mockClientFactory;
    private readonly Mock<ILogger<EmailService>> _mockLogger;
    private readonly IEmailService _emailService;

    public EmailServiceTests()
    {
        _configWrapper = new OptionsWrapper<EmailConfiguration>(EmailResources.GetConfiguration());
        _mockClientFactory = new Mock<ISmtpClientFactory>();
        _mockLogger = new Mock<ILogger<EmailService>>();
        _emailService = new EmailService(_configWrapper, _mockClientFactory.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task SendEmailAsync_ReturnsResultOk_WhenEmailSentSuccessfully()
    {
        var emailData = EmailResources.GetEmailData();

        var mockClient = new Mock<ISmtpClient>();

        _mockClientFactory
            .Setup(c => c.Create())
            .Returns(mockClient.Object);

        mockClient
            .Setup(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        mockClient
            .Setup(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        mockClient
            .Setup(c => c.SendAsync(It.IsAny<MimeMessage>()))
            .Returns(Task.FromResult(It.IsAny<string>()));

        mockClient
            .Setup(c => c.DisconnectAsync(It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var result = await _emailService.SendEmailAsync(EmailResources.GetEmailData());

        mockClient.Verify(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        mockClient.Verify(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        mockClient.Verify(c => c.SendAsync(It.IsAny<MimeMessage>()), Times.Once);
        mockClient.Verify(c => c.DisconnectAsync(It.IsAny<bool>()), Times.Once);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public async Task SendEmailAsync_ReturnsResultFail_WhenConnectionFails()
    {
        var emailData = EmailResources.GetEmailData();

        var mockClient = new Mock<ISmtpClient>();

        _mockClientFactory
            .Setup(c => c.Create())
            .Returns(mockClient.Object);

        mockClient
            .Setup(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ThrowsAsync(new IOException());

        mockClient
            .Setup(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        mockClient
            .Setup(c => c.SendAsync(It.IsAny<MimeMessage>()))
            .Returns(Task.FromResult(It.IsAny<string>()));

        mockClient
            .Setup(c => c.DisconnectAsync(It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var result = await _emailService.SendEmailAsync(EmailResources.GetEmailData());

        mockClient.Verify(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        mockClient.Verify(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockClient.Verify(c => c.SendAsync(It.IsAny<MimeMessage>()), Times.Never);
        mockClient.Verify(c => c.DisconnectAsync(It.IsAny<bool>()), Times.Never);

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task SendEmailAsync_ReturnsResultFail_WhenAuthenticationFails()
    {
        var emailData = EmailResources.GetEmailData();

        var mockClient = new Mock<ISmtpClient>();

        _mockClientFactory
            .Setup(c => c.Create())
            .Returns(mockClient.Object);

        mockClient
            .Setup(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        mockClient
            .Setup(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new AuthenticationException());

        mockClient
            .Setup(c => c.DisconnectAsync(It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var result = await _emailService.SendEmailAsync(EmailResources.GetEmailData());

        mockClient.Verify(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        mockClient.Verify(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        mockClient.Verify(c => c.SendAsync(It.IsAny<MimeMessage>()), Times.Never);
        mockClient.Verify(c => c.DisconnectAsync(It.IsAny<bool>()), Times.Never);

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task SendEmailAsync_ReturnsResultFail_WhenSendingEmailFails()
    {
        var emailData = EmailResources.GetEmailData();

        var mockClient = new Mock<ISmtpClient>();

        _mockClientFactory
            .Setup(c => c.Create())
            .Returns(mockClient.Object);

        mockClient
            .Setup(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        mockClient
            .Setup(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        mockClient
            .Setup(c => c.SendAsync(It.IsAny<MimeMessage>()))
            .ThrowsAsync(new Exception());

        mockClient
            .Setup(c => c.DisconnectAsync(It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var result = await _emailService.SendEmailAsync(EmailResources.GetEmailData());

        mockClient.Verify(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        mockClient.Verify(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        mockClient.Verify(c => c.SendAsync(It.IsAny<MimeMessage>()), Times.Once);
        mockClient.Verify(c => c.DisconnectAsync(It.IsAny<bool>()), Times.Never);

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task SendEmailAsync_ReturnsResultFail_WhenDisconnectingConnectionFails()
    {
        var emailData = EmailResources.GetEmailData();

        var mockClient = new Mock<ISmtpClient>();

        _mockClientFactory
            .Setup(c => c.Create())
            .Returns(mockClient.Object);

        mockClient
            .Setup(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        mockClient
            .Setup(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        mockClient
            .Setup(c => c.SendAsync(It.IsAny<MimeMessage>()))
            .Returns(Task.FromResult(It.IsAny<string>()));

        mockClient
            .Setup(c => c.DisconnectAsync(It.IsAny<bool>()))
            .ThrowsAsync(new ObjectDisposedException(nameof(_emailService)));

        var result = await _emailService.SendEmailAsync(emailData);

        mockClient.Verify(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        mockClient.Verify(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        mockClient.Verify(c => c.SendAsync(It.IsAny<MimeMessage>()), Times.Once);
        mockClient.Verify(c => c.DisconnectAsync(It.IsAny<bool>()), Times.Once);

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
    }
}
