using RestoreV2.Reader;
using RestoreV2;
try
{
    Parameters parameters = new(args);
    BackUpFile fileBackUp = new(parameters.BackUpFileName); // объект файла бекапа
    parameters.FilesCount = Convert.ToInt32(fileBackUp.ReadServiceInfo());   

    FilesArray filesArray = new (parameters.FilesCount);

    Threads threads = new Threads(filesArray.FilesToWrite, parameters, fileBackUp);

    Thread readFile = new Thread(threads.ReadFromFile);
    readFile.Start();

    Thread writeFile = new Thread(threads.WriteToFile);
    writeFile.Start();
    writeFile.Join();   
}
catch (Exception error)
{
    Exeptions exeption = new Exeptions(error);
}
