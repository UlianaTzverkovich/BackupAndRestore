using RestoreV2.Reader;
using RestoreV2;
try
{
    Parameters parameters = new(args);
    BackUpFile fileBackUp = new(parameters.BackUpFileName); // объект файла бекапа
    fileBackUp.ReadServiceInfo(parameters);   

    FilesArray filesArray = new (parameters.FilesCount);

    Threads threads = new Threads(filesArray.FilesToWrite, parameters, fileBackUp);

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
}
catch (Exception error)
{
    Exeptions exeption = new Exeptions(error);
}
