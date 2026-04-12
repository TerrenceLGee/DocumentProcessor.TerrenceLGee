using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;

public interface ITextService
{
    Task<Result> WriteContactsTextFileAsync(List<Contact> contacts, List<string> headerNames, string fileName);
}
