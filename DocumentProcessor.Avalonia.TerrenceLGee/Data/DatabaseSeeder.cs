using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Data;

public static class DatabaseSeeder
{

    public static async Task SeedDatabaseAsync(ContactDbContext context, IFileReader writer)
    {
        if (await context.Contacts.AnyAsync()) return;

        var contacts = writer.ReadContactsFromFile(FilePaths.FilePath);

        if (!contacts.IsSuccess || contacts.Value is null) return;

        await context.Contacts.AddRangeAsync(contacts.Value);
        await context.SaveChangesAsync();
    }
}
