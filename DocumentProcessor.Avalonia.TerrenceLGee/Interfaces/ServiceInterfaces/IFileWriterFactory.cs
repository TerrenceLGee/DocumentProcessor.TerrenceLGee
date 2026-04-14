namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;

public interface IFileWriterFactory
{
    IFileWriter GetWriter(string format);
}
