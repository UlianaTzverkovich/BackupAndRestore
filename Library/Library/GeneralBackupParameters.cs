using System.Reflection;
namespace Library;

public class GeneralBackupParameters
{ 
    public string FilesDirectory { get; set; } = string.Empty;
    public string BackUpFileName { get; set;} = string.Empty;
    public bool Recursive { get; set;} = false;
    public bool Compress { get; set;} = false;
    public bool Encryption { get; set;} = false;
    public string Password { get; set;} = string.Empty;
    public int FilesCount { get; set;} 
}