using CsvHelper.Configuration;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Helpers;

public class ContactMap : ClassMap<Contact>
{
    public ContactMap()
    {
        Map(c => c.FirstName).Index(0);
        Map(c => c.MiddleInitial).Index(1);
        Map(c => c.LastName).Index(2);
        Map(c => c.EmailAddress).Index(3);
        Map(c => c.TelephoneNumber).Index(4);
    }
}
