using DocumentProcessor.Avalonia.TerrenceLGee.Common.Extensions;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Pagination;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Parameters;
using DocumentProcessor.Avalonia.TerrenceLGee.DTOs;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using System.Linq;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Mappings;

public static class ToDto
{
    extension(Contact contact)
    {
        public RetrievedContactDto ToRetrievedContactDto()
        {
            return new RetrievedContactDto
            {
                Id = contact.Id,
                FirstName = contact.FirstName,
                MiddleInitial = contact.MiddleInitial,
                LastName = contact.LastName,
                EmailAddress = contact.EmailAddress,
                TelephoneNumber = contact.TelephoneNumber
            };
        }

        public CreateContactDto ToCreateContactDto()
        {
            return new CreateContactDto
            {
                FirstName = contact.FirstName,
                MiddleInitial = contact.MiddleInitial,
                LastName = contact.LastName,
                EmailAddress = contact.EmailAddress,
                TelephoneNumber = contact.TelephoneNumber
            };
        }
    }

    extension(PagedList<Contact> contacts)
    {
        public PagedList<RetrievedContactDto> ToPagedListOfRetrievedContactDto(PaginationParams paginationParams)
        {
            return contacts
                .Select(c => c.ToRetrievedContactDto())
                .ToPagedList(contacts.TotalEntities, paginationParams.Page, paginationParams.PageSize);
        }
    }
}
