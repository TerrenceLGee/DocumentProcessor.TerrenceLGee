using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using System.Collections.Generic;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;

public interface IFileReader
{
    IReadOnlyList<string> SupportedFormats { get; }
    Result<List<Contact>> ReadContactsFromFile(string filePath);
}
