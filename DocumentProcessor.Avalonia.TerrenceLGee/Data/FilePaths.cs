using System.IO;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Data;

public static class FilePaths
{
    private static string FileName = "contacts.xlsx";
    private static string CombinedPath = Path.Combine(Directory.GetCurrentDirectory(), FileName);
    public static string FilePath = CombinedPath;
}
