using DocumentProcessor.Avalonia.TerrenceLGee.Models;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Mappings;

public static class FromEntityToEntity
{
    extension(Contact fromContact)
    {
        public void FromContactToContact(Contact toContact)
        {
            toContact.Id = fromContact.Id;
            toContact.FirstName = fromContact.FirstName;
            toContact.MiddleInitial = fromContact.MiddleInitial;
            toContact.LastName = fromContact.LastName;
            toContact.EmailAddress = fromContact.EmailAddress;
            toContact.TelephoneNumber = fromContact.TelephoneNumber;
        }
    }
}
