using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestoreV2.Writer;
using RestoreV2.Reader;
namespace RestoreV2
{
    public class Threads
    {
        FileToWrite[] FilesToWrite; 
        List<FilePieces> FilePieces = new List<FilePieces>();
        BackUpFile BackUpFile;
        Parameters Params;
        int StandartPieceSize;
        float MemoryInUse = 0;
        float ByteToMB = 1048576;
        public Threads(FileToWrite[] FilesToWrite, Parameters Params, BackUpFile BackUpFile)
        {
            this.FilesToWrite = FilesToWrite;
            this.StandartPieceSize = Params.StandartPieceSize;
            this.BackUpFile = BackUpFile;
            this.Params = Params;
        }
        public void ReadFromFile()
        {
            int pieceToRead = 0;
            FileToWrite currentFile;
            for (int fileCounter = 0; fileCounter < Params.FilesCount; fileCounter++)
            {
                currentFile = FilesToWrite[fileCounter];
                currentFile.ShortFileName = BackUpFile.ReadServiceInfo();
                currentFile.FileSize = Convert.ToInt64(BackUpFile.ReadServiceInfo());
                long pieceCount = currentFile.FileSize / StandartPieceSize + 1;
                long lastPieceSize = currentFile.FileSize - (pieceCount - 1) * StandartPieceSize;  
                
                for (int currentPiece = 1; currentPiece <= pieceCount; currentPiece++)
                {
                    FilePieces piece = new FilePieces();

                    if (currentPiece == pieceCount) { pieceToRead = (int)lastPieceSize; }
                    else { pieceToRead = StandartPieceSize; }

                    byte[] fileBinaryData = BackUpFile.ReadFileContent(pieceToRead);

                    piece.PieceData = fileBinaryData;
                    if (!Params.Encryption & !Params.Archive)
                    {
                        piece.AllowToWrite = true; //разрешение на запись в конце обработки (архивация,шифрование)
                    }
                    while (MemoryInUse >= Params.MaxMemoryUse) { Thread.Sleep(Params.TimeToWait); } //ожидание освобождения памяти
                    FilePieces.Add(piece);
                    MemoryInUse = MemoryInUse + pieceToRead / ByteToMB;
                }

            }

        }

        public void WriteToFile()
        {                 
            int pos = 0;
            int indexArrayCorrection = 1;
            foreach (FileToWrite currentfile in FilesToWrite)
            {
                while (currentfile.FileSize == 0) { Thread.Sleep(Params.TimeToWait); }; //ожидание заполения массива с файлами
                currentfile.CreateFile(Params.FilesCreateDirectory + currentfile.ShortFileName);
                long pieceCount = currentfile.FileSize / StandartPieceSize;
                if (pieceCount * StandartPieceSize < currentfile.FileSize)
                {
                    pieceCount++;
                }

                int pieceCountInt = (int)pieceCount;
                
                for (int i = pos; i < pieceCountInt + pos; i++)
                {
                    while (i > FilePieces.Count - indexArrayCorrection) { Thread.Sleep(Params.TimeToWait); } //кусочек несчитался 
                    while (!FilePieces[i].AllowToWrite) { Thread.Sleep(Params.TimeToWait); } //кусочек недобработался 
                    if(currentfile.Writer != null) { currentfile.Writer.Write(FilePieces[i].PieceData); }
                    
                    float PieceSize = FilePieces[i].PieceData.Length;
                    FilePieces[i].PieceData = new byte[0]; // освобождение памяти
                    MemoryInUse = MemoryInUse - PieceSize / ByteToMB;
                }
                if (currentfile.Writer != null) { currentfile.Writer.Dispose(); }
                
                pos = pos + pieceCountInt;
            }
        }

    }
}
