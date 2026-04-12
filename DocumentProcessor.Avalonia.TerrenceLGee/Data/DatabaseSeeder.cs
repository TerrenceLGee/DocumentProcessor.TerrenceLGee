using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Data;

public static class DatabaseSeeder
{

    public static async Task SeedDatabaseAsync(ContactDbContext context, IXLService xlService)
    {
        if (await context.Contacts.AnyAsync()) return;

        var contacts = xlService.ReadXLFile(FilePaths.FilePath, FilePaths.WorksheetName);

        if (!contacts.IsSuccess || contacts.Value is null) return;

        await context.Contacts.AddRangeAsync(contacts.Value);
        await context.SaveChangesAsync();
    }
}
