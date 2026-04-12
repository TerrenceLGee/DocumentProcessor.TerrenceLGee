using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using System.Collections.Generic;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;

public interface IXLService
{
    Result<List<Contact>> ReadXLFile(string filePath, string sheetName);
    Result AddContactToXLFile(Contact contact, string filePath, string sheetName);
    Result UpdateContactInXLFile(Contact contact, string filePath, string sheetName);
    Result DeleteContactFromXLFile(int contactId, string filePath, string sheetName);
    Result WriteContactsXLFile(List<Contact> contacts, List<string> headerNames, string filePath, string sheetName); 
}
