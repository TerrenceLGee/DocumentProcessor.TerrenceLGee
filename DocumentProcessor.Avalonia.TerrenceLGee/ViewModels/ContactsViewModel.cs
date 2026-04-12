using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Parameters;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Data;
using DocumentProcessor.Avalonia.TerrenceLGee.DTOs;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Mappings;
using DocumentProcessor.Avalonia.TerrenceLGee.Messages;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using DocumentProcessor.Avalonia.TerrenceLGee.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
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
    private readonly IXLService _xlService;
    private readonly ITextService _textService;
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
    [NotifyPropertyChangedFor(nameof(TelephoneNumberErrors))]
    private string _telephoneNumber;

    public string? TelephoneNumberErrors => GetErrors(nameof(TelephoneNumber))
        .FirstOrDefault()?.ErrorMessage;

    [ObservableProperty]
    private string? _errorMessage;

    public ContactsViewModel(
        IContactService contactService, 
        IXLService xlService, 
        ITextService textService,
        IMessenger messenger)
    {
        _contactService = contactService;
        _xlService = xlService;
        _textService = textService;
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

            var addResult = _xlService.AddContactToXLFile(result.Value.FromRetrievedContactDto(),
                        FilePaths.FilePath, FilePaths.WorksheetName);

            await LoadContactsAsync();
            ClearFields();

            if (!addResult.IsSuccess)
            {
                box = MessageBoxManager
                        .GetMessageBoxStandard("Error", $"{result.Value.FirstName}{result.Value.LastName} " +
                        $"not added to the excel spreadsheet", ButtonEnum.Ok, Icon.Error,
                        null, WindowStartupLocation.CenterOwner);
                await box.ShowAsync();
            }
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

            var updatedResult = _xlService.UpdateContactInXLFile(contact.FromUpdateContactDto(),
                FilePaths.FilePath, FilePaths.WorksheetName);

            await LoadContactsAsync();
            ClearFields();

            if (!updatedResult.IsSuccess)
            {
                box = MessageBoxManager
                        .GetMessageBoxStandard("Error", $"{contact.FirstName}{contact.LastName} " +
                        $"not updated in the excel spreadsheet", ButtonEnum.Ok, Icon.Error,
                        null, WindowStartupLocation.CenterOwner);
                await box.ShowAsync();
            }
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

                var deletedResult = _xlService.DeleteContactFromXLFile(SelectedContact.Id,
                    FilePaths.FilePath, FilePaths.WorksheetName);

                await LoadContactsAsync();
                ClearFields();


                if (!deletedResult.IsSuccess)
                {
                    box = MessageBoxManager
                            .GetMessageBoxStandard("Error", $"{SelectedContact.FirstName}{SelectedContact.LastName} " +
                            $"not updated in the excel spreadsheet", ButtonEnum.Ok, Icon.Error,
                            null, WindowStartupLocation.CenterOwner);
                    await box.ShowAsync();
                }

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
    private async Task SaveToFileAsync(Visual? visual)
    {
        var topLevel = TopLevel.GetTopLevel(visual);
        if (topLevel is null) return;

        ErrorMessage = null;

        var xlsxFilter = new FilePickerFileType("Excel")
        {
            Patterns = ["*.xlsx"],
            MimeTypes = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"]
        };

        var textFilter = new FilePickerFileType("Text")
        {
            Patterns = ["*.txt"],
        };

        var options = new FilePickerSaveOptions
        {
            Title = "Save File",
            SuggestedFileName = "newFile",
            DefaultExtension = "xlsx",
            FileTypeChoices = [xlsxFilter, textFilter],
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
        var headerNames = new List<string>
        {
            "Id",
            "First Name",
            "Middle Initial",
            "Last Name",
            "Email Address",
            "Phone Number"
        };

        
        var sheetName = "ContactsSheet";

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

        var result = fileExtension switch
        {
            ".xlsx" => _xlService.WriteContactsXLFile(contactsToSave, headerNames, filePath, sheetName),
            ".txt" => await _textService.WriteContactsTextFileAsync(contactsToSave, headerNames, filePath),
            _ => Result.Fail("Invalid file format")
        };

        if (result.IsSuccess)
        {
            var box = MessageBoxManager
                    .GetMessageBoxStandard("Success",
                    $"File saved successfully", ButtonEnum.Ok, Icon.Success,
                    null, WindowStartupLocation.CenterOwner);
            await box.ShowAsync();
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
