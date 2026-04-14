namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;

public interface IFileWriterFactory
{
    IFileWriterService GetWriter(string format);
}
