using Library;
using static Backup.ThreadsManager;
namespace Backup.Threads;
public class WriteToBackup
{
    public WriteToBackup(){}
    public void Write()
    {
        int pos = 0;
        int indexArrayCorrection = 1;
        if (ThreadsManager.FilesList != null)
            foreach (CurrentFile currentFile in ThreadsManager.FilesList)
            {
                long pieceCount = currentFile.GetFilePieces(SystemRuntimeProp.StandartPieceSize);
                int pieceCountInt = (int)pieceCount;
                FileBackUp?.WriteServiceInfoForFile(currentFile, pieceCountInt);
                for (int currentPiecePosition = pos; currentPiecePosition < pieceCountInt + pos; currentPiecePosition++)
                {
                    while (ThreadsManager.FilePieces != null &&
                           currentPiecePosition > ThreadsManager.FilePieces.Count - indexArrayCorrection)
                    {
                        Thread.Sleep(SystemRuntimeProp.TimeToWait);
                    }

                    ; //кусочек несчитался 
                    while (ThreadsManager.FilePieces != null && !ThreadsManager.FilePieces[currentPiecePosition].AllowToWrite)
                    {
                        Thread.Sleep(SystemRuntimeProp.TimeToWait);
                    }

                    ; //кусочек недобработался                                        
                    var pieceData = ThreadsManager.FilePieces?[currentPiecePosition].PieceData;
                    if (pieceData != null)
                    {
                        FileBackUp?.WritePieceOfData(pieceData);
                        if (ThreadsManager.FilePieces != null)
                        {
                            float PieceSize = ThreadsManager.FilePieces[currentPiecePosition].PieceData.Length;
                            ThreadsManager.FilePieces[currentPiecePosition].PieceData = new byte[0]; // освобождение памяти
                            MemoryInUse = MemoryInUse - PieceSize / ByteToMB;
                        }
                    }
                }

                pos = pos + pieceCountInt;
            }
    }
    
}