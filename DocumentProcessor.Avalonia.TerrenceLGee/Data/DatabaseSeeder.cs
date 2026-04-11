using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Data;

public static class DatabaseSeeder
{
    private const string FileName = "contacts.xlsx";
    private static string FilePath = Path.Combine(Directory.GetCurrentDirectory(), FileName);
    private const string WorksheetName = "ContactsSheet";
    public static async Task SeedDatabaseAsync(ContactDbContext context, IXLService xlService)
    {
        if (await context.Contacts.AnyAsync()) return;

        var contacts = xlService.ReadXLFile(FilePath, WorksheetName);

        if (!contacts.IsSuccess || contacts.Value is null) return;

        await context.Contacts.AddRangeAsync(contacts.Value);
        await context.SaveChangesAsync();
    }
}
