using System.Globalization;
using System.Reflection;

namespace Library;

/// <summary>
/// Common class for parsing and setting commandline options 
/// </summary>
public class CommandLineParams
{
    public Dictionary<string, string> Parameters { get; } = new Dictionary<string, string>();
    public CommandLineParams() { }
}
    
