using DocumentProcessor.Avalonia.TerrenceLGee.DTOs;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Mappings;

public static class FromDto
{
    extension(CreateContactDto dto)
    {
        public Contact FromCreateContactDto()
        {
            return new Contact
            {
                FirstName = dto.FirstName,
                MiddleInitial = dto.MiddleInitial,
                LastName = dto.LastName,
                EmailAddress = dto.EmailAddress,
                TelephoneNumber = dto.TelephoneNumber
            };
        }
    }

    extension(UpdateContactDto dto)
    {
        public Contact FromUpdateContactDto()
        {
            return new Contact
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                MiddleInitial = dto.MiddleInitial,
                LastName = dto.LastName,
                EmailAddress = dto.EmailAddress,
                TelephoneNumber = dto.TelephoneNumber
            };
        }
    }
}
