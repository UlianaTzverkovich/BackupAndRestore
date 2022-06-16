using Backup.Threads;
using Backup.ThreadsMethods;
using Library;

namespace Backup;
public class ThreadsManager
{
    public static List<CurrentFile> FilesList = null!;
    public static List<FilePieces> FilePieces = null!;
    public FileToWrite FileBackUp { get; }
    public static float MemoryInUse = 0;
    public static float ByteToMB = 1048576;
    public ThreadsManager(List<CurrentFile> files,FileToWrite fileBackUp)
    {
        FilesList = files;
        FilePieces = new List<FilePieces>();
        FileBackUp = fileBackUp;
        //BinaryWriter = new BinaryWriter();
        ReadFromFile readFromFile = new ReadFromFile();
        Thread readFile = new Thread(readFromFile.Read);
        readFile.Start();

        if (ParamManager.ServiceInfo!.Compress | ParamManager.ServiceInfo.Encryption)
        {
            ProcessingFile processing = new ProcessingFile();
            for (int currentThreadNum = 0; currentThreadNum < SystemRuntimeProp.MaxProcessingThreads; currentThreadNum++)
            {
                Thread dataProcessing = new Thread(processing.ProcessingFilePiece); //TODO разделить на отдельные методы архивацию и шифрование
                dataProcessing.Start();
            }
        }
        WriteToBackup writeToFile = new WriteToBackup(FileBackUp.StreamForWrite);
        Thread writeFile = new Thread(writeToFile.Writer);
        writeFile.Start();
        writeFile.Join();
    }
}