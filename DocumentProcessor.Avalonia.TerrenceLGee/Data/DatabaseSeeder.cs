using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Data;

public static class DatabaseSeeder
{
    public static async Task SeedDatabaseAsync(ContactDbContext context)
    {
        if (await context.Contacts.AnyAsync()) return;

        var contacts = new[]
        {
            new Contact
            {
                FirstName = "Homer",
                MiddleInitial = "J.",
                LastName = "Simpson",
                EmailAddress = "HSimpson@example.com",
                TelephoneNumber = "123-456-7890"
            },
            new Contact
            {
                FirstName = "Marjorie",
                MiddleInitial = "B.",
                LastName = "Simpson",
                EmailAddress = "MSimpson@example.com",
                TelephoneNumber = "123-456-7891"
            },
            new Contact
            {
                FirstName = "Bartholomew",
                MiddleInitial = "J.",
                LastName = "Simpson",
                EmailAddress = "ElBarto@example.com",
                TelephoneNumber = "123-456-7892"
            },
            new Contact
            {
                FirstName = "Lisa",
                MiddleInitial = "M.",
                LastName = "Simpson",
                EmailAddress = "LSimpson@example.com",
                TelephoneNumber = "123-456-7893"
            },
            new Contact
            {
                FirstName = "Margaret",
                MiddleInitial = "L.",
                LastName = "Simpson",
                EmailAddress = "MaggieS@example.com",
                TelephoneNumber = "123-456-7894"
            },
            new Contact
            {
                FirstName = "Abraham",
                MiddleInitial = "J.",
                LastName = "Simpson",
                EmailAddress = "GrandpaSimpson@example.com",
                TelephoneNumber = "123-456-7895"
            },
            new Contact
            {
                FirstName = "Lenny",
                MiddleInitial = "",
                LastName = "Leonard",
                EmailAddress = "LeonardL@example.com",
                TelephoneNumber = "123-456-7896"
            },
            new Contact
            {
                FirstName = "Carlton",
                MiddleInitial = "C.",
                LastName = "Carlson",
                EmailAddress = "Carlson@example.com",
                TelephoneNumber = "123-456-7897"
            },
            new Contact
            {
                FirstName = "Herschel",
                MiddleInitial = "S.",
                LastName = "Krustofsky",
                EmailAddress = "KrustyTheClown@example.com",
                TelephoneNumber = "123-456-7898"
            },
            new Contact
            {
                FirstName = "Barnard",
                MiddleInitial = "A.",
                LastName = "Gumble",
                EmailAddress = "BaGumble@example.com",
                TelephoneNumber = "123-456-7899"
            },
            new Contact
            {
                FirstName = "Robert",
                MiddleInitial = "U.",
                LastName = "Terwilliger",
                EmailAddress = "SideShowBob@example.com",
                TelephoneNumber = "123-456-7819"
            },
            new Contact
            {
                FirstName = "Moammar",
                MiddleInitial = "M.",
                LastName = "Szyslak",
                EmailAddress = "BartenderMoe@example.com",
                TelephoneNumber = "123-456-7829"
            },
            new Contact
            {
                FirstName = "Waylon",
                MiddleInitial = "J.",
                LastName = "Smithers",
                EmailAddress = "SmitherW@example.com",
                TelephoneNumber = "123-456-7839"
            },
            new Contact
            {
                FirstName = "Charles",
                MiddleInitial = "M",
                LastName = "Burns",
                EmailAddress = "MrBurns@example.com",
                TelephoneNumber = "123-456-7849"
            },
            new Contact
            {
                FirstName = "Nelson",
                MiddleInitial = "M",
                LastName = "Muntz",
                EmailAddress = "BullyNo1@example.com",
                TelephoneNumber = "123-456-7859"
            },
        };

        await context.Contacts.AddRangeAsync(contacts);
        await context.SaveChangesAsync();
    }
}
