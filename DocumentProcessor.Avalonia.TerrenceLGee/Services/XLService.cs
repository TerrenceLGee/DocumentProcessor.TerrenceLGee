using ClosedXML.Excel;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Helpers;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Services;

public class XLService : IFileWriter, IFileReader
{
    private readonly ILogger<XLService> _logger;

    public XLService(ILogger<XLService> logger)
    {
        _logger = logger;
    }

    public Result<List<Contact>> ReadContactsFromFile(string filePath)
    {
        var errorMessage = string.Empty;
        try
        {
            var contacts = new List<Contact>();
            
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);

                var rows = worksheet.RowsUsed();

                foreach (var row in rows)
                {
                    contacts.Add(new Contact
                    {
                        FirstName = row.Cell(1).Value.ToString(),
                        MiddleInitial = row.Cell(2).Value.ToString(),
                        LastName = row.Cell(3).Value.ToString(),
                        EmailAddress = row.Cell(4).Value.ToString(),
                        TelephoneNumber = row.Cell(5).Value.ToString()
                    });
                }

                return Result<List<Contact>>.Ok(contacts);
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"{LogMessageHelper.GetMessageForLogging(nameof(XLService), nameof(ReadContactsFromFile))}" +
                $"There was an unexpected error reading file: {filePath}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return Result<List<Contact>>.Fail($"There was an unexpected error reading file: {filePath}");
        }
    }

    public Result WriteContactsToFile(List<Contact> contacts, string filePath)
    {
        var errorMessage = string.Empty;
        try
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet(1);

                var headerNames = HeaderHelper.GetHeaderNames();

                for (int col = 0; col < headerNames.Count; col++)
                {
                    worksheet.Cell(1, col + 1).Value = headerNames[col];
                }

                worksheet.Cell(2, 1).InsertData(contacts);

                workbook.SaveAs(filePath);

                return Result.Ok();
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"{LogMessageHelper.GetMessageForLogging(nameof(XLService), nameof(WriteContactsToFile))}" +
                $"There was an unexpected error writing to the file: {filePath}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return Result.Fail($"There was an unexpected error writing to the file: {filePath}");
        }
    }

    public IReadOnlyList<string> SupportedFormats => new List<string> { "xlsx", ".xlsx" };
}
