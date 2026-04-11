using ClosedXML.Excel;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using System.Collections.Generic;
using System.Linq;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Services;

public class XLService : IXLService
{
    public Result<List<Contact>> ReadXLFile(string filePath, string sheetName)
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

    public Result WriteXLFile()
    {
        throw new System.NotImplementedException();
    }
}
