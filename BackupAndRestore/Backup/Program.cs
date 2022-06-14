using Backup;
using Library; 
try
{ 
    ParamManager paramMan = new (args);
    GetFilesManager GetFiles = new GetFilesManager();
    string[] files = GetFiles.GetFiles();
    FilesList filesList = new(files);
    
    BackUpFile fileBackUp = new(); // объект файла бекапа
    fileBackUp.WriteServiceInfo();
    
    ThreadsManager threads = new ThreadsManager(filesList.FileList, fileBackUp);
    if (fileBackUp.Writer != null) fileBackUp.Writer.Dispose();
}
catch (Exception error)
{
    Exeptions exeption = new Exeptions(error);
}