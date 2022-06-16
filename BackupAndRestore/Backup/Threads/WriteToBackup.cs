using Library;
using static Backup.ThreadsManager;
namespace Backup.Threads;

public class WriteToBackup
{
    private BinaryWriter _binaryWriter;

    public WriteToBackup(Stream streamForWrite)
    {
        _binaryWriter = new(streamForWrite);
    }

    public void Writer()
    {
        int pos = 0;
        int indexArrayCorrection = 1;
        if (ThreadsManager.FilesList != null)
            foreach (CurrentFile currentFile in ThreadsManager.FilesList)
            {
                long pieceCount = currentFile.GetFilePieces(SystemRuntimeProp.StandartPieceSize);
                int pieceCountInt = (int)pieceCount;
                WriteServiceInfoForFile(currentFile, pieceCountInt);
                for (int currentPiecePosition = pos; currentPiecePosition < pieceCountInt + pos; currentPiecePosition++)
                {
                    while (ThreadsManager.FilePieces != null &&
                           currentPiecePosition > ThreadsManager.FilePieces.Count - indexArrayCorrection)
                    {
                        Thread.Sleep(SystemRuntimeProp.TimeToWait);
                    }

                    ; //кусочек несчитался 
                    while (ThreadsManager.FilePieces != null &&
                           !ThreadsManager.FilePieces[currentPiecePosition].AllowToWrite)
                    {
                        Thread.Sleep(SystemRuntimeProp.TimeToWait);
                    }

                    ; //кусочек недобработался                                        
                    var pieceData = ThreadsManager.FilePieces?[currentPiecePosition].PieceData;
                    if (pieceData != null)
                    {
                        WritePieceOfData(pieceData);
                        if (ThreadsManager.FilePieces != null)
                        {
                            float PieceSize = ThreadsManager.FilePieces[currentPiecePosition].PieceData.Length;
                            ThreadsManager.FilePieces[currentPiecePosition].PieceData =
                                new byte[0]; // освобождение памяти
                            MemoryInUse = MemoryInUse - PieceSize / ByteToMB;
                        }
                    }
                }

                pos = pos + pieceCountInt;
            }
    }

    public void WriteServiceInfo()
    {
        _binaryWriter.Write(ParamManager.ServiceInfo.FilesCount.ToString());
        _binaryWriter.Write(ParamManager.ServiceInfo.Compress);
        _binaryWriter.Write(SystemRuntimeProp.StandartPieceSize);
        _binaryWriter.Write(ParamManager.ServiceInfo.Encryption);
    }

    public void WriteServiceInfoForFile(CurrentFile file, int pieceCount)
    {
        _binaryWriter.Write(file.ShortFileName);
        _binaryWriter.Write(file.FileSize.ToString());
        _binaryWriter.Write(pieceCount);
    }

    public void WritePieceOfData(byte[] data)
    {
        _binaryWriter.Write(data.Length.ToString());
        _binaryWriter.Write(data);

    }
}