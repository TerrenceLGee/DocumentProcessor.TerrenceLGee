using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using System.Collections.Generic;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;

public interface IPdfService
{
    Result WriteContactsToPdfFile(List<Contact> contacts, List<string> headerNames, string fileName);
}
