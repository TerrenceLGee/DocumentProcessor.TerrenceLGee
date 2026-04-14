using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Services;

public class FileWriterFactory : IFileWriterFactory
{
    private readonly IEnumerable<IFileWriter> _writers;

    public FileWriterFactory(IEnumerable<IFileWriter> writers)
    {
        _writers = writers;
    }

    public IFileWriter GetWriter(string format)
    {
        return _writers.FirstOrDefault(w =>
        w.SupportedFormats.Contains(format, StringComparer.OrdinalIgnoreCase))
            ?? throw new KeyNotFoundException($"No writer found for format: {format}");
    }
}
