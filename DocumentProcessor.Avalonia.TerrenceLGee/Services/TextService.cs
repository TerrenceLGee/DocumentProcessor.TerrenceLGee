using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Services;

public class TextService : ITextService
{
    private readonly ILogger<TextService> _logger;

    public TextService(ILogger<TextService> logger)
    {
        _logger = logger;
    }
    public async Task<Result> WriteContactsTextFileAsync(List<Contact> contacts, List<string> headerNames, string fileName)
    {
        var errorMessage = string.Empty;

        try
        {
            using (var writer = new StreamWriter(fileName))
            {
                await writer.WriteLineAsync($"{headerNames[0],-5} " +
                    $"{headerNames[1],-15} " +
                    $"{headerNames[2],-15} " +
                    $"{headerNames[3], -15} " +
                    $"{headerNames[4], -30} " +
                    $"{headerNames[5], -15}");

                await writer.WriteLineAsync(new string('-', 100));

                foreach (var contact in contacts)
                {
                    await writer.WriteLineAsync(
                        $"{contact.Id, -5} " +
                        $"{contact.FirstName, -15} " +
                        $"{contact.MiddleInitial, -15} " +
                        $"{contact.LastName, -15} " +
                        $"{contact.EmailAddress, -30} " +
                        $"{contact.TelephoneNumber, -15}");
                }
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            errorMessage = $"{GetMessageForLogging(nameof(WriteContactsTextFileAsync))}" +
                $"There was an unexpected error saving the file: {fileName}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return Result.Fail($"There was an unexpected error saving the file: {fileName}");
        }
    }

    private string GetMessageForLogging(string methodName)
    {
        return $"\nClass: {nameof(TextService)}\n" +
            $"Method: {nameof(methodName)}";
    }
}
