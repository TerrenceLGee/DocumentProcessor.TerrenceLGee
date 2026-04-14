namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;

public interface IFileReaderFactory
{
    IFileReader GetReader(string format);
}
