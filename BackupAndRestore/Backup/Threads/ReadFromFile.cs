using Library;
using static Backup.ThreadsManager;
namespace Backup.ThreadsMethods;

public class ReadFromFile
{
    public ReadFromFile() { }
    public void Read()
    {
        int pieceToRead;
        if (ThreadsManager.FilesList != null)
            foreach (CurrentFile currentFile in ThreadsManager.FilesList)
            {
                long pieceCount = currentFile.GetFilePieces(SystemRuntimeProp.StandartPieceSize);
                long lastPieceSize = currentFile.FileSize - (pieceCount - 1) * SystemRuntimeProp.StandartPieceSize;
                FileToRead fileToRead = new FileToRead();
                BinaryReader binaryReader = new BinaryReader(fileToRead.StreamForRead);
                for (int currentPiecePosition = 1; currentPiecePosition <= pieceCount; currentPiecePosition++)
                {
                    FilePieces piece = new FilePieces();
                    if (currentPiecePosition == pieceCount)
                    {
                        pieceToRead = (int)lastPieceSize;
                    }
                    else
                    {
                        pieceToRead = SystemRuntimeProp.StandartPieceSize;
                    }

                    byte[] FileBinaryData = binaryReader.ReadBytes(pieceToRead);
                    piece.PieceData = FileBinaryData;
                    if (!ParamManager.ServiceInfo!.Encryption & !ParamManager.ServiceInfo.Compress)
                    {
                        piece.AllowToWrite = true; //разрешение на запись в конце обработки (архивация,шифрование)
                    }

                    while (MemoryInUse >= SystemRuntimeProp.MaxMemoryUse)
                    {
                        Thread.Sleep(SystemRuntimeProp.TimeToWait);
                    } //ожидание освобождения памяти

                    ThreadsManager.FilePieces.Add(piece);
                    MemoryInUse = MemoryInUse + pieceToRead / ByteToMB;
                }
            }
    }
}
    
