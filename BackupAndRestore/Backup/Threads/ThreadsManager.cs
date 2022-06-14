using Backup.Threads;
using Backup.ThreadsMethods;
using Library;

namespace Backup;
public class ThreadsManager
{
    public static List<CurrentFile>? FilesList;
    public static List<FilePieces>? FilePieces;
    public static BackUpFile? FileBackUp { get;  private set; }
    public static float MemoryInUse = 0;
    public static float ByteToMB = 1048576;
    public ThreadsManager(List<CurrentFile>? files,BackUpFile? fileBackUp)
    {
        FilesList = files;
        FileBackUp = fileBackUp;
        ReadFromFile readFromFile = new ReadFromFile();
        Thread readFile = new Thread(readFromFile.Read);
        readFile.Start();

        if (ParamManager.ServiceInfo!.Compress | ParamManager.ServiceInfo.Encryption)
        {
            ProcessingFile processing = new ProcessingFile();
            for (int currentThreadNum = 0; currentThreadNum < SystemRuntimeProp.MaxProcessingThreads; currentThreadNum++)
            {
                Thread dataProcessing = new Thread(processing.ProcessingFilePiece); //TODO добавить разные методы на архивацию и шифрование
                dataProcessing.Start();
            }
        }
        WriteToBackup writeToFile = new WriteToBackup();
        Thread writeFile = new Thread(writeToFile.Write);
        writeFile.Start();
        writeFile.Join();
    }
}