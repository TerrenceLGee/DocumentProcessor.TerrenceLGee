using CsvHelper;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Helpers;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Services;

public class CsvService : IFileWriterService, IFileReaderService
{
    private readonly ILogger<CsvService> _logger;

    public CsvService(ILogger<CsvService> logger)
    {
        _logger = logger;
    }

    public Result WriteContactsToFile(List<Contact> contacts, string fileName)
    {
        var errorMessage = string.Empty;
        try
        {
            using var writer = new StreamWriter(fileName);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(contacts);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            errorMessage = $"{LogMessageHelper.GetMessageForLogging(nameof(CsvService), nameof(WriteContactsToFile))}" +
                $"There was an unexpected error writing to file: {fileName}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return Result.Fail($"There was an unexpected error writing to file: {fileName}");
        }
    }

    public Result<List<Contact>> ReadContactsFromFile(string filePath)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<string> SupportedFormats => new List<string> { "csv", ".csv" };
}
