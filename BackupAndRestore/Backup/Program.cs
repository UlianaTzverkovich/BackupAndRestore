using Backup;
using Library; 

try
{ 
    ParamManager paramMan = new (args);
    GetFilesManager GetFiles = new GetFilesManager();
    string[] files = GetFiles.GetFiles();
    FilesList filesList = new(files);
    
    FileToWrite fileBackUp = new(); // объект файла бекапа
    //WriteToBackup writeToBackup = new WriteToBackup(fileBackUp.Writer);
    //writeToBackup.WriteServiceInfo();
    
    ThreadsManager threads = new ThreadsManager(filesList.FileList, fileBackUp);
    //if (fileBackUp.Writer != null) fileBackUp.Writer.Dispose();
}
catch (Exception error)
{
    Exeptions exeption = new Exeptions(error);
}