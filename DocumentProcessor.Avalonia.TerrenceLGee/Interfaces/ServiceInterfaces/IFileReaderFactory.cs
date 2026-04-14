namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;

public interface IFileReaderFactory
{
    IFileReaderService GetReader(string format);
}
