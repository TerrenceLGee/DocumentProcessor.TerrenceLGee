using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using System.Collections.Generic;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;

public interface IFileWriter
{
    IReadOnlyList<string> SupportedFormats { get; }
    Result WriteContactsToFile(List<Contact> contacts, string filePath);
}
