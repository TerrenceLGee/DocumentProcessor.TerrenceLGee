namespace DocumentProcessor.Avalonia.TerrenceLGee.Helpers;

public static class LogMessageHelper
{
    public static string GetMessageForLogging(string className, string methodName)
    {
        return $"\nClass: {className}\n" +
            $"Method: {methodName}\n";
    }
}
