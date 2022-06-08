using BackupV2.Writer;
using BackupV2.Reader;
using BackupV2;
try
{      
    Parameters parameters = new(args);
    BackUpFile fileBackUp = new(parameters.BackUpFileName); // объект файла бекапа

    EnumerationOptions enumerationOptions = new EnumerationOptions();
    enumerationOptions.RecurseSubdirectories = parameters.Recursive;

    string[] files = Directory.GetFiles(parameters.SearchDirectory, parameters.SearchMask, enumerationOptions);
    FilesList filesList = new(files, parameters.SearchDirectory);

    fileBackUp.WriteServiceInfo(filesList.FileList.Count);

    Threads threads = new Threads(filesList.FileList, parameters, fileBackUp);    

    Thread readFile = new Thread(threads.ReadFromFile);
    readFile.Start();

    Thread writeFile = new Thread(threads.WriteToFile);
    writeFile.Start();
   
    writeFile.Join();
    fileBackUp.Writer.Dispose();
}

catch (Exception error)
{
    Exeptions exeption = new Exeptions (error);
}










