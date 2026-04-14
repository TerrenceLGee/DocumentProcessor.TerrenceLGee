using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Helpers;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Services;

public class PdfService : IFileWriter
{
    private readonly ILogger<PdfService> _logger;

    public PdfService(ILogger<PdfService> logger)
    {
        _logger = logger;
    }

    public Result WriteContactsToFile(List<Contact> contacts, string fileName)
    {
        var errorMessage = string.Empty;
        try
        {
            var headerNames = HeaderHelper.GetHeaderNames();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(6));

                    page.Header()
                    .Text("Contacts Report")
                    .SemiBold()
                    .FontSize(12)
                    .FontColor(Colors.Black);

                    page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(10);

                        x.Item()
                        .Text($"Total contacts: {contacts.Count}")
                        .FontSize(6);

                        x.Item()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                foreach (var name in headerNames)
                                {
                                    header.Cell()
                                    .Element(CellStyle)
                                    .BorderBottom(2)
                                    .Padding(6)
                                    .Text($"{name}")
                                    .Bold();
                                }

                                static IContainer CellStyle(IContainer container) =>
                                container
                                .DefaultTextStyle(x => x.FontSize(6)
                                .SemiBold()
                                .FontColor(Colors.Black))
                                .PaddingVertical(5)
                                .Background(Colors.White);
                            });

                            foreach (var contact in contacts)
                            {
                                table.Cell().Padding(6).Text($"{contact.Id}");
                                table.Cell().Padding(6).Text($"{contact.FirstName}");
                                table.Cell().Padding(6).Text($"{contact.MiddleInitial}");
                                table.Cell().Padding(6).Text($"{contact.LastName}");
                                table.Cell().Padding(6).Text($"{contact.EmailAddress}");
                                table.Cell().Padding(6).Text($"{contact.TelephoneNumber}");
                            }
                        });
                    });
                });
            }).GeneratePdf(fileName);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            errorMessage = $"{LogMessageHelper.GetMessageForLogging(nameof(PdfService), nameof(WriteContactsToFile))}" +
                $"There was an unexpected error saving the file: {fileName}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return Result.Fail($"There was an unexpected error saving the file: {fileName}");
        }
    }

    public IReadOnlyList<string> SupportedFormats => new List<string> { "pdf", ".pdf" };
}
