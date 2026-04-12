using ClosedXML.Excel;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Services;

public class XLService : IXLService
{
    private readonly ILogger<XLService> _logger;

    public XLService(ILogger<XLService> logger)
    {
        _logger = logger;
    }

    public Result<List<Contact>> ReadXLFile(string filePath, string sheetName)
    {
        var errorMessage = string.Empty;
        try
        {
            var contacts = new List<Contact>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(sheetName);

                var rows = worksheet.RowsUsed();

                foreach (var row in rows.Skip(1))
                {
                    contacts.Add(new Contact
                    {
                        Id = int.TryParse(row.Cell(1).Value.ToString(), out var id) ? id : 0,
                        FirstName = row.Cell(2).Value.ToString(),
                        MiddleInitial = row.Cell(3).Value.ToString(),
                        LastName = row.Cell(4).Value.ToString(),
                        EmailAddress = row.Cell(5).Value.ToString(),
                        TelephoneNumber = row.Cell(6).Value.ToString()
                    });
                }

                return Result<List<Contact>>.Ok(contacts);
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"{GetMessageForLogging(nameof(ReadXLFile))}" +
                $"There was an unexpected error reading file: {filePath}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return Result<List<Contact>>.Fail($"There was an unexpected error reading file: {filePath}");
        }
    }

    public Result WriteContactsXLFile(List<Contact> contacts, List<string> headerNames, string filePath, string sheetName)
    {
        var errorMessage = string.Empty;
        try
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet(sheetName);

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
            errorMessage = $"{GetMessageForLogging(nameof(WriteContactsXLFile))}" +
                $"There was an unexpected error writing to the file: {filePath}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return Result.Fail($"There was an unexpected error writing to the file: {filePath}");
        }
    }

    public Result AddContactToXLFile(Contact contact, string filePath, string sheetName)
    {
        var errorMessage = string.Empty;

        try
        {
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(sheetName);

                if (worksheet is null)
                {
                    return Result.Fail($"Unable to retrieve the work sheet: {sheetName}");
                }

                var lastRow = worksheet.LastRowUsed();

                if (lastRow is null)
                {
                    return Result.Fail($"Unable to retrieve the last used row in work sheet: {sheetName}");
                }

                var indexOfLastRow = lastRow.RowNumber();

                var cell = worksheet.Cell(indexOfLastRow + 1, 1);

                cell.InsertData(new[] { contact });

                workbook.SaveAs(filePath);

                return Result.Ok();
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"{GetMessageForLogging(nameof(AddContactToXLFile))}There was an unexpected " +
                $"error adding {contact.FirstName}{contact.LastName} to file " +
                $"{filePath}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return Result.Fail($"There was an unexpected error adding " +
                $"{contact.FirstName}{contact.LastName} to file {filePath}");
        }
    }

    public Result UpdateContactInXLFile(Contact contact, string filePath, string sheetName)
    {
        var errorMessage = string.Empty;
        try
        {
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(sheetName);

                var rows = worksheet.RowsUsed();

                foreach (var row in rows.Skip(1))
                {
                    var idToCheck = int.TryParse(row.Cell(1).Value.ToString(), out var id) ? id : 0;

                    if (contact.Id == idToCheck)
                    {
                        var rowNumber = row.RowNumber();

                        var cell = worksheet.Cell(rowNumber, 1);

                        cell.InsertData(new[] { contact });
                        break;
                    }
                }

                workbook.SaveAs(filePath);

                return Result.Ok();
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"{GetMessageForLogging(nameof(UpdateContactInXLFile))}There was an unexpected " +
                $"error updating {contact.FirstName}{contact.LastName} in file " +
                $"{filePath}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return Result.Fail($"There was an unexpected error updating " +
                $"{contact.FirstName}{contact.LastName} in file {filePath}");
        }
    }

    public Result DeleteContactFromXLFile(int contactId, string filePath, string sheetName)
    {
        var errorMessage = string.Empty;
        try
        {
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(sheetName);

                var rows = worksheet.RowsUsed();

                foreach (var row in rows.Skip(1))
                {
                    var idToCheck = int.TryParse(row.Cell(1).Value.ToString(), out var id) ? id : 0;

                    if (contactId == idToCheck)
                    {
                        row.Delete();
                        break;
                    }
                }

                workbook.SaveAs(filePath);
            }
            return Result.Ok();
        }
        catch (Exception ex)
        {
            errorMessage = $"{GetMessageForLogging(nameof(DeleteContactFromXLFile))}There was an unexpected " +
                $"error deleting the contact from file " +
                $"{filePath}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return Result.Fail($"There was an unexpected error deleting " +
                $"the contact from file {filePath}");
        }
    }

    private string GetMessageForLogging(string methodName)
    {
        return $"\nClass: {nameof(XLService)}\n" +
            $"Method: {methodName}\n";
    }
}
