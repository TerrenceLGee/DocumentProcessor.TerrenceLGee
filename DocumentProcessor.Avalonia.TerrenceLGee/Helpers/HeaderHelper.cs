using System.Collections.Generic;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Helpers;

public static class HeaderHelper
{
    public static List<string> GetHeaderNames()
    {
        return new List<string>
        {
            "Id",
            "First Name",
            "Middle Initial",
            "Last Name",
            "Email Address",
            "Telephone Number"
        };
    }
}
