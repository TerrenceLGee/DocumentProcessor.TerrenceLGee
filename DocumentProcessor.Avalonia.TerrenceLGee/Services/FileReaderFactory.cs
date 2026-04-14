using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Services;

public class FileReaderFactory : IFileReaderFactory
{
    private readonly IEnumerable<IFileReader> _readers;

    public FileReaderFactory(IEnumerable<IFileReader> readers)
    {
        _readers = readers;
    }

    public IFileReader GetReader(string format)
    {
        return _readers.FirstOrDefault(r => 
        r.SupportedFormats.Contains(format, StringComparer.OrdinalIgnoreCase))
            ?? throw new NotSupportedException($"No reader found for format: {format}");
    }
}
