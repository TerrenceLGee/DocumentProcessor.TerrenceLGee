using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Parameters;
using DocumentProcessor.Avalonia.TerrenceLGee.DTOs;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.ViewModels;

public partial class ContactsViewModel : ObservableValidator
{
    private readonly IContactService _contactService;
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

    public ContactsViewModel(IContactService contactService, IMessenger messenger)
    {
        _contactService = contactService;
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

        if (result.IsSuccess)
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Success", "Contact successfully added", ButtonEnum.Ok, Icon.Success,
                null, WindowStartupLocation.CenterOwner);
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
}
