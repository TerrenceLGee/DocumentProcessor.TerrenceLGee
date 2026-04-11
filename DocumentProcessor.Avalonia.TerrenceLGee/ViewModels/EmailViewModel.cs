using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentProcessor.Avalonia.TerrenceLGee.DTOs;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Messages;
using DocumentProcessor.Avalonia.TerrenceLGee.Models.EmailModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.ViewModels;

public partial class EmailViewModel : ObservableValidator
{
    private readonly IEmailService _emailService;
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private RetrievedContactDto _contactToEmail;

    [ObservableProperty]
    [Required(ErrorMessage = "Recipient name is required.")]
    [MaxLength(100, ErrorMessage = "Recipient name cannot exceed 100 characters.")]
    [NotifyPropertyChangedFor(nameof(RecipientNameErrors))]
    private string _receiverName = string.Empty;

    public string? RecipientNameErrors => GetErrors(nameof(ReceiverName))
        .FirstOrDefault()?.ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Recipient email address is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [NotifyPropertyChangedFor(nameof(RecipientEmailErrors))]
    private string _receiverEmail = string.Empty;

    public string? RecipientEmailErrors => GetErrors(nameof(ReceiverEmail))
        .FirstOrDefault()?.ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Email subject is required.")]
    [MaxLength(200, ErrorMessage = "Email subject cannot exceed 200 characters.")]
    [NotifyPropertyChangedFor(nameof(EmailSubjectErrors))]
    private string _subject = string.Empty;

    public string? EmailSubjectErrors => GetErrors(nameof(Subject))
        .FirstOrDefault()?.ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Email body is required.")]
    [MinLength(1, ErrorMessage = "Email body must be at least 1 character.")]
    [NotifyPropertyChangedFor(nameof(EmailBodyErrors))]
    public string _body = string.Empty;

    public string? EmailBodyErrors => GetErrors(nameof(Subject))
        .FirstOrDefault()?.ErrorMessage;

    [ObservableProperty]
    private string? _errorMessage;

    public EmailViewModel(IEmailService emailService, IMessenger messenger, RetrievedContactDto contactToEmail)
    {
        _emailService = emailService;
        _messenger = messenger;
        _contactToEmail = contactToEmail;
        _receiverName = $"{contactToEmail.FirstName} {contactToEmail.LastName}";
        _receiverEmail = contactToEmail.EmailAddress;
    }

    [RelayCommand]
    private async Task SendEmailAsync()
    {
        ErrorMessage = null;

        ClearErrors();

        ValidateProperty(ReceiverName, nameof(ReceiverName));
        ValidateProperty(ReceiverEmail, nameof(ReceiverEmail));
        ValidateProperty(Subject, nameof(Subject));
        ValidateProperty(Body, nameof(Body));

        if (HasErrors)
        {
            return;
        }

        var emailData = new EmailData
        {
            ReceiverName = ReceiverName,
            ReceiverEmail = ReceiverEmail,
            Subject = Subject,
            Body = Body
        };

        var result = await _emailService.SendEmailAsync(emailData);

        if (result.IsSuccess)
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Success", $"Email successfully send to {ReceiverEmail}", ButtonEnum.Ok, Icon.Success,
                null, WindowStartupLocation.CenterOwner);

            await box.ShowAsync();
            ClearFields();
        }
        else
        {
            ErrorMessage = result.ErrorMessage;
            var box = MessageBoxManager
                .GetMessageBoxStandard("Error", $"{ErrorMessage}", ButtonEnum.Ok, Icon.Error,
                null, WindowStartupLocation.CenterOwner);

            await box.ShowAsync();
            ClearFields();
        }
    }

    [RelayCommand]
    private void GoBack()
    {
        _messenger.Send(new NavigateBackToPreviousPageMessage());
    }

    [RelayCommand]
    private void ClearFields()
    {
        ReceiverName = string.Empty;
        ReceiverEmail = string.Empty;
        Subject = string.Empty;
        Body = string.Empty;
    }
}
