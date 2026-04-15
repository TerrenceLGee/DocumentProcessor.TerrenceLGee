using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Parameters;
using DocumentProcessor.Avalonia.TerrenceLGee.DTOs;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Mappings;
using DocumentProcessor.Avalonia.TerrenceLGee.Messages;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using DocumentProcessor.Avalonia.TerrenceLGee.Models.EmailModels;
using Microsoft.Extensions.Options;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.ViewModels;

public partial class ContactsViewModel : ObservableValidator
{
    private readonly IContactService _contactService;
    private readonly IEmailService _emailService;
    private readonly EmailConfiguration _emailConfiguration;
    private readonly IFileWriterFactory _writerFactory;
    private readonly IFileReaderFactory _readerFactory;

    private readonly IMessenger _messenger;

    [ObservableProperty]
    private ObservableCollection<RetrievedContactDto> _contacts = [];

    [ObservableProperty]
    private bool _isLoading;
    [ObservableProperty]
    private RetrievedContactDto? _selectedContact;

    [ObservableProperty]
    private int _page = 1;
    [ObservableProperty]
    private int _pageSize = 10;
    [ObservableProperty]
    private int _totalPages;
    [ObservableProperty]
    private bool _hasPreviousPage;
    [ObservableProperty]
    private bool _hasNextPage;

    [ObservableProperty]
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    [NotifyPropertyChangedFor(nameof(FirstNameErrors))]
    private string _firstName = string.Empty;

    public string? FirstNameErrors => GetErrors(nameof(FirstName))
        .FirstOrDefault()?.ErrorMessage;

    [ObservableProperty]
    [MaxLength(2, ErrorMessage = "Middle initial cannot exceed 2 characters (The initial and a period).")]
    [NotifyPropertyChangedFor(nameof(MiddleInitialErrors))]
    private string? _middleInitial;

    public string? MiddleInitialErrors => GetErrors(nameof(MiddleInitial))
        .FirstOrDefault()?.ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    [NotifyPropertyChangedFor(nameof(LastNameErrors))]
    private string _lastName = string.Empty;

    public string? LastNameErrors => GetErrors(nameof(LastName))
        .FirstOrDefault()?.ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [NotifyPropertyChangedFor(nameof(EmailErrors))]
    private string _emailAddress = string.Empty;

    public string? EmailErrors => GetErrors(nameof(EmailAddress))
        .FirstOrDefault()?.ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Telephone number is required.")]
    [RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$", ErrorMessage = "Invalid phone number.")]
    [NotifyPropertyChangedFor(nameof(TelephoneNumberErrors))]
    private string _telephoneNumber;

    public string? TelephoneNumberErrors => GetErrors(nameof(TelephoneNumber))
        .FirstOrDefault()?.ErrorMessage;

    [ObservableProperty]
    private string? _errorMessage;

    public ContactsViewModel(
        IContactService contactService,
        IEmailService emailService,
        IFileWriterFactory writerFactory,
        IFileReaderFactory readerFactory,
        IOptions<EmailConfiguration> configuration,
        IMessenger messenger)
    {
        _contactService = contactService;
        _emailService = emailService;
        _writerFactory = writerFactory;
        _readerFactory = readerFactory;
        _emailConfiguration = configuration.Value;
        _messenger = messenger;
        LoadContactsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadContactsAsync()
    {
        Page = 1;
        await FetchContactsAsync();
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (!HasNextPage) return;
        Page++;
        await FetchContactsAsync();
    }

    [RelayCommand]
    private async Task PreviousPageAsync()
    {
        if (!HasPreviousPage) return;
        Page--;
        await FetchContactsAsync();
    }

    private async Task FetchContactsAsync()
    {
        IsLoading = true;

        var paginationParams = new PaginationParams
        {
            Page = Page,
            PageSize = PageSize
        };

        var result = await _contactService.GetContactsAsync(paginationParams);

        if (!result.IsFailure && result.Value is not null)
        {
            Contacts.Clear();

            foreach (var contact in result.Value)
            {
                Contacts.Add(contact);
            }

            TotalPages = result.Value.TotalPages;
            HasNextPage = Page < TotalPages;
            HasPreviousPage = Page > 1;
        }

        IsLoading = false;
    }

    [RelayCommand]
    private async Task AddContactAsync()
    {
        ErrorMessage = null;

        ClearErrors();

        ValidateProperty(FirstName, nameof(FirstName));
        ValidateProperty(MiddleInitial, nameof(MiddleInitial));
        ValidateProperty(LastName, nameof(LastName));
        ValidateProperty(EmailAddress, nameof(EmailAddress));
        ValidateProperty(TelephoneNumber, nameof(TelephoneNumber));

        if (HasErrors)
        {
            return;
        }

        var contact = new CreateContactDto
        {
            FirstName = FirstName,
            MiddleInitial = MiddleInitial,
            LastName = LastName,
            EmailAddress = EmailAddress,
            TelephoneNumber = TelephoneNumber
        };

        var result = await _contactService.AddContactAsync(contact);

        if (result.Value is null)
        {
            ErrorMessage = result.ErrorMessage;
            var box = MessageBoxManager
                .GetMessageBoxStandard("Error", $"{ErrorMessage}", ButtonEnum.Ok, Icon.Error,
                null, WindowStartupLocation.CenterOwner);

            await box.ShowAsync();
            ClearFields();
            return;
        }

        if (result.IsSuccess)
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Success", $"{result.Value.FirstName} {result.Value.LastName} added",
                ButtonEnum.Ok, Icon.Success,
                null, WindowStartupLocation.CenterOwner);
            var response = await box.ShowAsync();

            await LoadContactsAsync();
            ClearFields();
        }
    }

    [RelayCommand]
    private async Task AddContactsFromFileAsync(Visual? visual)
    {
        var topLevel = TopLevel.GetTopLevel(visual);
        if (topLevel is null) return;

        ErrorMessage = null;

        var xlsxFilter = new FilePickerFileType(".xlsx")
        {
            Patterns = ["*.xlsx"],
            MimeTypes = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"]
        };

        var textFilter = new FilePickerFileType(".txt")
        {
            Patterns = ["*.txt"]
        };

        var csvFilter = new FilePickerFileType(".csv")
        {
            Patterns = ["*.csv"]
        };

        var options = new FilePickerOpenOptions
        {
            Title = "Open File",
            FileTypeFilter = [xlsxFilter, csvFilter, textFilter]
        };

        var filePath = await topLevel
            .StorageProvider
            .OpenFilePickerAsync(options);

        if (filePath is null) return;

        var fullPath = filePath[0].Path.AbsolutePath.ToString();

        if (fullPath is null) return;

        var fileName = filePath[0].Name;

        if (fileName is null) return;

        var fileParts = fileName.Split('.');

        if (fileParts.Count() < 2) return;

        var fileExtension = fileParts[1];

        var reader = _readerFactory.GetReader(fileExtension);

        var result = reader.ReadContactsFromFile(fullPath);

        if (result.IsSuccess)
        {
            if (result.Value is not null)
            {
                var contacts = result.Value
                    .Select(c => c.ToCreateContactDto())
                    .ToList();

                var addContactsResult = await _contactService.AddContactsAsync(contacts);

                if (addContactsResult.IsSuccess)
                {
                    var box = MessageBoxManager
                        .GetMessageBoxStandard("Success", "Contacts saved successfully", ButtonEnum.Ok, Icon.Success,
                        null, WindowStartupLocation.CenterOwner);
                    await box.ShowAsync();

                    await LoadContactsAsync();
                }
            }
            else
            {
                ErrorMessage = $"No contacts were able to be read from file: {fileName}";
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Error", $"{ErrorMessage}", ButtonEnum.Ok, Icon.Error,
                    null, WindowStartupLocation.CenterOwner);
            }
        }
        else
        {
            ErrorMessage = result.ErrorMessage;
            var box = MessageBoxManager
                .GetMessageBoxStandard("Error", $"{ErrorMessage}", ButtonEnum.Ok, Icon.Error,
                null, WindowStartupLocation.CenterOwner);

            await box.ShowAsync();
        }
    }

    [RelayCommand]
    private async Task UpdateContactAsync()
    {
        ErrorMessage = null;

        ClearErrors();

        ValidateProperty(FirstName, nameof(FirstName));
        ValidateProperty(MiddleInitial, nameof(MiddleInitial));
        ValidateProperty(LastName, nameof(LastName));
        ValidateProperty(EmailAddress, nameof(EmailAddress));
        ValidateProperty(TelephoneNumber, nameof(TelephoneNumber));

        if (HasErrors)
        {
            return;
        }

        if (SelectedContact is null)
        {
            return;
        }


        var contact = new UpdateContactDto
        {
            Id = SelectedContact.Id,
            FirstName = FirstName,
            MiddleInitial = MiddleInitial,
            LastName = LastName,
            EmailAddress = EmailAddress,
            TelephoneNumber = TelephoneNumber
        };

        var result = await _contactService.UpdateContactAsync(contact);

        if (result.IsSuccess)
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Success", $"{contact.FirstName} {contact.LastName} successfully updated",
                ButtonEnum.Ok, Icon.Success, null, WindowStartupLocation.CenterOwner);

            await box.ShowAsync();

            await LoadContactsAsync();
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
    private async Task DeleteContactAsync()
    {
        if (SelectedContact is not null)
        {
            ErrorMessage = null;

            var result = await _contactService.DeleteContactAsync(SelectedContact.Id);

            if (result.IsSuccess)
            {
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Success",
                    $"{SelectedContact.FirstName} {SelectedContact.LastName} deleted successfully", ButtonEnum.Ok, Icon.Success,
                    null, WindowStartupLocation.CenterOwner);
                await box.ShowAsync();

                await LoadContactsAsync();
                ClearFields();

                SelectedContact = null;
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Error", $"{ErrorMessage}", ButtonEnum.Ok, Icon.Error,
                    null, WindowStartupLocation.CenterOwner);

                await box.ShowAsync();

                ClearFields();
                SelectedContact = null;
            }
        }
    }

    [RelayCommand]
    private async Task GenerateReportAsync(Visual? visual)
    {
        var topLevel = TopLevel.GetTopLevel(visual);
        if (topLevel is null) return;

        ErrorMessage = null;

        var xlsxFilter = new FilePickerFileType(".xlsx")
        {
            Patterns = ["*.xlsx"],
            MimeTypes = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"]
        };

        var textFilter = new FilePickerFileType(".txt")
        {
            Patterns = ["*.txt"]
        };

        var pdfFilter = new FilePickerFileType(".pdf")
        {
            Patterns = ["*.pdf"]
        };

        var csvFilter = new FilePickerFileType(".csv")
        {
            Patterns = ["*.csv"]
        };

        var options = new FilePickerSaveOptions
        {
            Title = "Save File",
            SuggestedFileName = "contacts",
            DefaultExtension = "xlsx",
            FileTypeChoices = [pdfFilter, xlsxFilter, csvFilter, textFilter],
            ShowOverwritePrompt = true
        };

        var fileName = await topLevel
            .StorageProvider
            .SaveFilePickerAsync(options);

        if (fileName is null) return;

        var filePath = fileName.Path.LocalPath;
        var page = 1;
        var pageSize = 10;
        var totalPages = int.MaxValue;
        var contactsToSave = new List<Contact>();

        while (page <= totalPages)
        {
            var paginationParams = new PaginationParams
            {
                Page = page,
                PageSize = pageSize
            };

            var contactsResult = await _contactService.GetContactsAsync(paginationParams);

            if (contactsResult.IsSuccess && contactsResult.Value is not null)
            {
                var contacts = contactsResult.Value;
                totalPages = contacts.TotalPages;

                foreach (var contact in contacts)
                {
                    contactsToSave.Add(contact.FromRetrievedContactDto());
                }
            }
            else
            {
                break;
            }

            page++;
        }

        var fileExtension = Path.GetExtension(filePath).ToLower();

        var writer = _writerFactory.GetWriter(fileExtension);

        var result = writer.WriteContactsToFile(contactsToSave, filePath);

        if (result.IsSuccess)
        {
            var box = MessageBoxManager
                    .GetMessageBoxStandard("Success",
                    $"Report generated successfully\nEmail report to yourself?", ButtonEnum.YesNo, Icon.Success,
                    null, WindowStartupLocation.CenterOwner);
            var response = await box.ShowAsync();

            if (response == ButtonResult.Yes)
            {
                var emailData = new EmailData
                {
                    ReceiverName = _emailConfiguration.SenderName,
                    ReceiverEmail = _emailConfiguration.SenderEmail,
                    Subject = $"Contacts report for {DateTime.Now}",
                    Body = "Lastest report of your saved contacts. See attached file",
                    Attachments = [new Attachment { FilePath = filePath}]
                };

                var emailResult = await _emailService.SendEmailAsync(emailData);

                if (emailResult.IsSuccess)
                {
                    box = MessageBoxManager
                        .GetMessageBoxStandard("Success", $"Report successfully sent to {emailData.ReceiverEmail}", ButtonEnum.Ok,
                        Icon.Success, null, WindowStartupLocation.CenterOwner);

                    await box.ShowAsync();
                }
                else
                {
                    ErrorMessage = emailResult.ErrorMessage;
                    box = MessageBoxManager
                        .GetMessageBoxStandard("Error", $"{ErrorMessage}", ButtonEnum.Ok, Icon.Error, 
                        null, WindowStartupLocation.CenterOwner);

                    await box.ShowAsync();
                }
            }
        }
        else
        {
            ErrorMessage = result.ErrorMessage;
            var box = MessageBoxManager
                .GetMessageBoxStandard("Error", $"{ErrorMessage}", ButtonEnum.Ok, Icon.Error,
                null, WindowStartupLocation.CenterOwner);

            await box.ShowAsync();
        }
    }

    [RelayCommand]
    private void SendEmail()
    {
        if (SelectedContact is not null)
        {
            _messenger.Send(new SendEmailMessage(SelectedContact));
        }
        ClearFields();
    }

    [RelayCommand]
    private void ClearFields()
    {
        FirstName = string.Empty;
        MiddleInitial = null;
        LastName = string.Empty;
        EmailAddress = string.Empty;
        TelephoneNumber = string.Empty;
    }

    partial void OnSelectedContactChanged(RetrievedContactDto? value)
    {
        if (value is not null)
        {
            FirstName = value.FirstName;
            MiddleInitial = value.MiddleInitial;
            LastName = value.LastName;
            EmailAddress = value.EmailAddress;
            TelephoneNumber = value.TelephoneNumber;
        }
    }

    [RelayCommand]
    private void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
}
