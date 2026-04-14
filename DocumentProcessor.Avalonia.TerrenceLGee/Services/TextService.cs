using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Helpers;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Services;

public class TextService : ITextService
{
    private readonly ILogger<TextService> _logger;

    public TextService(ILogger<TextService> logger)
    {
        _logger = logger;
    }
    public Result WriteContactsTextFile(List<Contact> contacts, string fileName)
    {
        var errorMessage = string.Empty;

        try
        {
            var headerNames = HeaderHelper.GetHeaderNames();

            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLineAsync($"{headerNames[0],-5} " +
                    $"{headerNames[1],-15} " +
                    $"{headerNames[2],-15} " +
                    $"{headerNames[3],-15} " +
                    $"{headerNames[4],-30} " +
                    $"{headerNames[5],-15}");

                writer.WriteLineAsync(new string('-', 100));

                foreach (var contact in contacts)
                {
                    writer.WriteLineAsync(
                        $"{contact.Id,-5} " +
                        $"{contact.FirstName,-15} " +
                        $"{contact.MiddleInitial,-15} " +
                        $"{contact.LastName,-15} " +
                        $"{contact.EmailAddress,-30} " +
                        $"{contact.TelephoneNumber,-15}");
                }
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            errorMessage = $"{LogMessageHelper.GetMessageForLogging(nameof(TextService), nameof(WriteContactsTextFile))}" +
                $"There was an unexpected error saving the file: {fileName}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return Result.Fail($"There was an unexpected error saving the file: {fileName}");
        }
    }
}
