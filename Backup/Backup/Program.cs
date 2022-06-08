
try
{
    BackUpMaker.BackUp FileBackUp = new(args[1]);

    int position = args[0].IndexOf("*."); //определяю переда ли маска в параметр
    if (position < 0)
    {
        FileBackUp.pathSearch = args[0];
    }
    else
    {
        FileBackUp.pathSearch = args[0].Substring(0, position);
        FileBackUp.mask = args[0].Substring(position);
    }

    int paramCount = args.Length; //если их всего 2 , то доп параметров нет
    if (paramCount > 2) //перебор массива параметров TODO: вынести в  метод
    {
        int i = paramCount;
        while (i > 2)
        {
            i--;
            string param = args[i];
            if (param == "--recursive")
            {
                FileBackUp.recursive = true;
            }
            else if (param == "--compress")
            {
                FileBackUp.compress = true;
            }
            else
            {
                FileBackUp.encryptKey = args[i++];
            }
        }
    }
    EnumerationOptions EnumerationOptions = new EnumerationOptions();
    EnumerationOptions.RecurseSubdirectories = FileBackUp.recursive;

    string[] files = Directory.GetFiles(FileBackUp.pathSearch, FileBackUp.mask, EnumerationOptions);

    int ArFilesLength = files.Length;
    FileBackUp.SumFiles = ArFilesLength;
    FileStream BakFileStream = File.Open(args[1], FileMode.Create);
    BinaryWriter BakbinWriter = new BinaryWriter(BakFileStream);
   // FileBackUp.OpenStreamWrite();

    foreach (string file in files)
    {
        using (FileStream fsSource = new FileStream(file,
            FileMode.Open, FileAccess.Read))
        {
            string ShortFileName = file.Substring(0, FileBackUp.pathSearch.Length);
            Backup.FileData FileData = new Backup.FileData(file.Length,ShortFileName, BakFileStream, FileBackUp.SumFiles, BakbinWriter); 
            Thread WriteToBackUp = new Thread(FileBackUp.WriteFile(fsSource));
            WriteToBackUp.Start();
            // Read the source file into a byte array.
            
        }
    }
    //bakbinWriter.Close();
    //bakfilestream.Close();
    FileData.Dispose();
    //BakbinWriter.Dispose();
    //BakFileStream.Dispose();
}


catch (Exception e)
{
    Console.WriteLine("The process failed: {0}", e.ToString());
}
   
    


        
    




