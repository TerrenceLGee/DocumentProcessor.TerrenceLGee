using CsvHelper;
using CsvHelper.Configuration;
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

public class CsvService : IFileWriter, IFileReader
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
        var errorMessage = string.Empty;
        try
        {
            var contacts = new List<Contact>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<ContactMap>();

            var contactRecords = csv.GetRecords<Contact>();

            foreach (var contact in contactRecords)
            {
                contacts.Add(contact);
            }

            return Result<List<Contact>>.Ok(contacts);
        }
        catch (Exception ex)
        {
            errorMessage = $"{LogMessageHelper.GetMessageForLogging(nameof(CsvService), nameof(WriteContactsToFile))}" +
                $"There was an unexpected error reading from file: {filePath}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return Result<List<Contact>>.Fail($"There was an unexpected error reading file: {filePath}");
        }
    }

    public IReadOnlyList<string> SupportedFormats => new List<string> { "csv", ".csv" };
}
