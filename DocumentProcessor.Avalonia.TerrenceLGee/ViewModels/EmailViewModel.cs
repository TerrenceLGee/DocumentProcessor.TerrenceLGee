using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentProcessor.Avalonia.TerrenceLGee.DTOs;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Messages;
using DocumentProcessor.Avalonia.TerrenceLGee.Models.EmailModels;
using MailKit.Net.Smtp;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.ViewModels;

public partial class EmailViewModel : ObservableValidator
{
    private readonly IEmailService _emailService;
    private readonly IRetryService _retryService;
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

    public ObservableCollection<Attachment> Attachments { get; set; } = [];

    [ObservableProperty]
    private Attachment? _selectedAttachment;

    [ObservableProperty]
    private List<Attachment> _attachmentFilePaths;

    [ObservableProperty]
    private string? _filePath;

    public EmailViewModel(
        IEmailService emailService,
        IRetryService retryService,
        IMessenger messenger, 
        RetrievedContactDto contactToEmail)
    {
        _emailService = emailService;
        _retryService = retryService;
        _messenger = messenger;
        _contactToEmail = contactToEmail;
        _receiverName = $"{contactToEmail.FirstName} {contactToEmail.LastName}";
        _receiverEmail = contactToEmail.EmailAddress;
        _attachmentFilePaths = [];
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

        AddAttachments();

        var emailData = new EmailData
        {
            ReceiverName = ReceiverName,
            ReceiverEmail = ReceiverEmail,
            Subject = (!string.IsNullOrEmpty(Subject)) ? Subject : "(no subject)",
            Body = Body,
            Attachments = AttachmentFilePaths
        };

        try
        {
            var result = await _retryService.ExecuteAsync(async () => await _emailService.SendEmailAsync(emailData),
                3,
                TimeSpan.FromMilliseconds(1000),
                ex => ex is SmtpProtocolException);

            if (result.IsSuccess)
            {
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Success", $"Email successfully send to {ReceiverEmail}", ButtonEnum.Ok, Icon.Success,
                    null, WindowStartupLocation.CenterOwner);

                await box.ShowAsync();
                ClearAttachments();
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
        catch (Exception)
        {
            ErrorMessage = $"Unexpected error attempting to send email to {ReceiverEmail}";
            var box = MessageBoxManager
                .GetMessageBoxStandard("Error", $"{ErrorMessage}", ButtonEnum.Ok, Icon.Error,
                null, WindowStartupLocation.CenterOwner);
        } 
    }

    [RelayCommand]
    private async Task AddAttachmentAsync(Visual? visual)
    {
        var topLevel = TopLevel.GetTopLevel(visual);
        if (topLevel is null) return;

        var options = new FilePickerOpenOptions
        {
            Title = "Add Attachment",
            AllowMultiple = true
        };

        var paths = await topLevel
            .StorageProvider
            .OpenFilePickerAsync(options);

        if (paths.Count == 0 || paths is null) return;

        foreach (var path in paths)
        {
            var tempPath = path.Path.PathAndQuery.ToString();
            var FilePath = tempPath.Replace("%20", " ");
            var attachment = new Attachment { FilePath = FilePath };
            Attachments.Add(attachment);
        }
    }

    private void AddAttachments()
    {
        if (Attachments.Count > 0)
        {
            foreach (var attachment in Attachments)
            {
                AttachmentFilePaths.Add(attachment);
            }
        }
    }

    [RelayCommand]
    private void RemoveAttachment()
    {
        if (SelectedAttachment is not null)
        {
            Attachments.Remove(SelectedAttachment);
            SelectedAttachment = null;
        }
    }

    [RelayCommand]
    private void ClearAttachments()
    {
        Attachments.Clear();
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
