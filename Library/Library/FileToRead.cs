namespace Library;

public class FileToRead
{
    public FileStream StreamForRead;

    public FileToRead()
    {
        StreamForRead = File.Open(GeneralBackupParameters.BackUpFileName, FileMode.Open);
    }
}