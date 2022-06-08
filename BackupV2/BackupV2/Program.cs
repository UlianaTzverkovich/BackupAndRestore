
try
{   
    
    BackupV2.Parameters Params = new(args);
    BackupV2.Writer.BackUpFile FileBackUp = new(Params.BackUpFileName); // объект файла бекапа

    EnumerationOptions EnumerationOptions = new EnumerationOptions();
    EnumerationOptions.RecurseSubdirectories = Params.Recursive;

    string[] files = Directory.GetFiles(Params.SearchDirectory, Params.SearchMask, EnumerationOptions);
    BackupV2.Reader.FilesList FilesList = new(files, Params.SearchDirectory);

    FileBackUp.WriteServiceInfo(FilesList.FileList.Count);

    BackupV2.Reader.Threads Threads = new BackupV2.Reader.Threads(FilesList.FileList, Params, FileBackUp);    

    Thread ReadFile = new Thread(Threads.ReadFromFile);
    ReadFile.Start();

    Thread WriteFile = new Thread(Threads.WriteToFile);
    WriteFile.Start();

    while (WriteFile.IsAlive || ReadFile.IsAlive)
   { 
        
   };
        FileBackUp.Writer.Dispose();
}

catch (Exception error)
{
    BackupV2.Exeptions Exeption = new BackupV2.Exeptions (error);
}










