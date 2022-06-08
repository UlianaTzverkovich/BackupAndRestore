using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackupV2.Reader;

namespace BackupV2
{
    public class Threads
    {
        List<FileToRead> FileList;
        List<FilePieces> FilePieces = new List<FilePieces>();
        Writer.BackUpFile BackUpFile;
        Parameters Params;
        int StandartPieceSize;
        float MemoryInUse = 0;
        float ByteToMB = 1048576;
        public Threads(List<FileToRead> fileList, Parameters Params, Writer.BackUpFile backUpFile)
        {
            this.FileList = fileList;
            this.StandartPieceSize = Params.StandartPieceSize;
            this.BackUpFile = backUpFile;
            this.Params = Params;
        }
        public void ReadFromFile()
        {
            int pieceToRead;
            foreach (FileToRead currentfile in FileList)
            {
                long pieceCount = currentfile.FileSize / StandartPieceSize + 1;
                long lastPieceSize = currentfile.FileSize - (pieceCount - 1) * StandartPieceSize;
                FileStream filestream = File.Open(currentfile.FullFileName, FileMode.Open);
                BinaryReader fileReader = new BinaryReader(filestream);

                for (int i = 1; i <= pieceCount; i++)
                {
                    FilePieces piece = new FilePieces();

                    if (i == pieceCount) { pieceToRead = (int)lastPieceSize; }
                    else { pieceToRead = StandartPieceSize; }

                    byte[] FileBinaryData = fileReader.ReadBytes(pieceToRead);

                    piece.PieceData = FileBinaryData;
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
            foreach (FileToRead currentfile in FileList)
            {
                long pieceCount = currentfile.FileSize / StandartPieceSize;
                if (pieceCount * StandartPieceSize < currentfile.FileSize)
                {
                    pieceCount++;

                };
                int pieceCountInt = (int)pieceCount;
                BackUpFile.WriteServiceInfoForFile(currentfile);

                for (int i = pos; i < pieceCountInt + pos; i++)
                {
                    while (i > FilePieces.Count - indexArrayCorrection) { Thread.Sleep(Params.TimeToWait); }; //кусочек несчитался 
                    while (!FilePieces[i].AllowToWrite) { Thread.Sleep(Params.TimeToWait); }; //кусочек недобработался 
                    BackUpFile.Writer.Write(FilePieces[i].PieceData);
                    float PieceSize = FilePieces[i].PieceData.Length;
                    FilePieces[i].PieceData = new byte[0]; // освобождение памяти
                    MemoryInUse = MemoryInUse - PieceSize / ByteToMB;
                }
                pos = pos + pieceCountInt;

            }
        }


    }
}
