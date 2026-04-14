using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using System.Collections.Generic;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;

public interface IXLService
{
    Result<List<Contact>> ReadXLFile(string filePath);
    Result WriteContactsXLFile(List<Contact> contacts, string filePath); 
}
