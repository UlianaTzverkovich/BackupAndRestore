using BackupV2.Writer;
using BackupV2.Reader;
using BackupV2;
try
{
    Parameters parameters = new(args);
    //parameters.

    EnumerationOptions enumerationOptions = new EnumerationOptions();
    enumerationOptions.RecurseSubdirectories = parameters.Recursive;

    string[] files = Directory.GetFiles(parameters.SearchDirectory, parameters.SearchMask, enumerationOptions);
    FilesList filesList = new(files, parameters.SearchDirectory);
    //using 
    BackUpFile fileBackUp = new(parameters.BackUpFileName); // объект файла бекапа

    fileBackUp.WriteServiceInfo(filesList.FileList.Count, parameters);

    Threads threads = new Threads(filesList.FileList, parameters, fileBackUp);

    Thread readFile = new Thread(threads.ReadFromFile);
    readFile.Start();

    if (parameters.Archive | parameters.Encryption)
    {
        for (int currentThreadNum = 0; currentThreadNum < parameters.MaxProcessingThreads; currentThreadNum++)
        {
            Thread dataProcessing = new Thread(threads.ProcessingFilePiece);
            dataProcessing.Start();
        }
    }  

    Thread writeFile = new Thread(threads.WriteToFile);
    writeFile.Start();

    writeFile.Join();

    fileBackUp.Writer.Dispose();

}

catch (Exception error)
{
    Exeptions exeption = new Exeptions(error);
}










