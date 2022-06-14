using Library;

namespace Backup;

public class GetFilesManager
{
    public GetFilesManager() { }
    public string[] GetFiles()
    {
        EnumerationOptions enumerationOptions = new EnumerationOptions();
        string[] files = new string[] { };
        if (ParamManager.ServiceInfo != null)
        {
            enumerationOptions.RecurseSubdirectories = ParamManager.ServiceInfo.Recursive;
            files = Directory.GetFiles(ParamManager.ServiceInfo.FilesDirectory, ParamManager.ServiceInfo.SearchMask!, enumerationOptions);
            ParamManager.ServiceInfo.FilesCount = files.Length;
        }
        return files;
    }
}