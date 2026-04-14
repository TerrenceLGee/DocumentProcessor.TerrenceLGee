using DocumentProcessor.Avalonia.TerrenceLGee.Common.Pagination;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Parameters;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.DTOs;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.RepositoryInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Mappings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Services;

public class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;

    public ContactService(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }
    public async Task<Result<RetrievedContactDto?>> AddContactAsync(CreateContactDto contact)
    {
        var result = await _contactRepository.AddContactAsync(contact.FromCreateContactDto());

        if (result is null)
        {
            return Result<RetrievedContactDto?>.Fail("Unable to add contact.");
        }

        return Result<RetrievedContactDto?>.Ok(result.ToRetrievedContactDto());
    }

    public async Task<Result> AddContactsAsync(List<CreateContactDto> contacts)
    {
        var result = await _contactRepository.AddContactsAsync(contacts.Select(c => c.FromCreateContactDto()).ToList());

        if (!result)
        {
            return Result.Fail("Unable to add contacts.");
        }

        return Result.Ok();
    }

    public async Task<Result<bool>> UpdateContactAsync(UpdateContactDto contact)
    {
        var result = await _contactRepository.UpdateContactAsync(contact.FromUpdateContactDto());

        if (!result)
        {
            return Result<bool>.Fail($"Unable to update contact {contact.Id}");
        }

        return Result<bool>.Ok(result);
    }

    public async Task<Result<bool>> DeleteContactAsync(int contactId)
    {
        var result = await _contactRepository.DeleteContactAsync(contactId);

        if (!result)
        {
            return Result<bool>.Fail($"Unable to delete contact {contactId}");
        }

        return Result<bool>.Ok(result);
    }

    public async Task<Result<RetrievedContactDto?>> GetContactAsync(int contactId)
    {
        var result = await _contactRepository.GetContactAsync(contactId);

        if (result is null)
        {
            return Result<RetrievedContactDto?>.Fail($"Unable to retrieve contact {contactId}");
        }

        return Result<RetrievedContactDto?>.Ok(result.ToRetrievedContactDto());
    }

    public async Task<Result<PagedList<RetrievedContactDto>>> GetContactsAsync(PaginationParams paginationParams)
    {
        var result = await _contactRepository.GetContactsAsync(paginationParams);

        if (result.Count == 0)
        {
            return Result<PagedList<RetrievedContactDto>>.Fail("Unable to retrieve contacts");
        }

        return Result<PagedList<RetrievedContactDto>>.Ok(result.ToPagedListOfRetrievedContactDto(paginationParams));
    }

   
}
