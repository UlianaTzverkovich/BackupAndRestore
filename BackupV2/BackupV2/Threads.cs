using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupV2.Reader
{
    public class Threads
    {
        List<FileToRead> FileList;
        int StandartPieceSize;
        List<FilePieces> FilePieces = new List<FilePieces>();
        Writer.BackUpFile BackUpFile;
        Parameters Params;
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
            int PieceToRead;
            foreach (FileToRead currentfile in FileList)
            {
                long PieceCount = currentfile.FileSize / StandartPieceSize + 1;
                long LastPieceSize = currentfile.FileSize - (PieceCount - 1) * StandartPieceSize;
                FileStream filestream = File.Open(currentfile.FullFileName, FileMode.Open);
                BinaryReader FileReader = new BinaryReader(filestream);

                for (int i = 1; i <= PieceCount; i++)
                {
                    FilePieces Piece = new FilePieces();

                    if (i == PieceCount) { PieceToRead = (int)LastPieceSize; }
                    else { PieceToRead = StandartPieceSize; }

                    byte[] FileBinaryData = FileReader.ReadBytes(PieceToRead);

                    Piece.PieceData = FileBinaryData;
                    if (!Params.Encryption & !Params.Archive) 
                    {
                        Piece.AllowToWrite = true; //разрешение на запись в конце обработки (архивация,шифрование)
                    }                    
                    while(MemoryInUse >= Params.MaxMemoryUse) { Thread.Sleep(Params.TimeToWait); } //ожидание освобождения памяти
                    FilePieces.Add(Piece);
                    MemoryInUse = MemoryInUse + PieceToRead / ByteToMB;
 
                }
            }
        }

        public void WriteToFile()
        {
            int Pos = 0;
            int IndexArrayCorrection = 1;
            foreach (FileToRead currentfile in FileList)
            {
                long PieceCount = currentfile.FileSize / StandartPieceSize;
                if (PieceCount * StandartPieceSize < currentfile.FileSize)
                {
                    PieceCount++;
                    
                };
                int PieceCountInt = (int)PieceCount;
                BackUpFile.WriteServiceInfoForFile(currentfile);

                for (int i = Pos; i < PieceCountInt + Pos; i++)
                {                    
                    while (i > FilePieces.Count - IndexArrayCorrection ) { Thread.Sleep(Params.TimeToWait); }; //кусочек несчитался 
                    while (!FilePieces[i].AllowToWrite) { Thread.Sleep(Params.TimeToWait); }; //кусочек недобработался 
                    BackUpFile.Writer.Write(FilePieces[i].PieceData);
                    float PieceSize = FilePieces[i].PieceData.Length;
                    FilePieces[i].PieceData = new byte[0]; // освобождение памяти
                    MemoryInUse = MemoryInUse - PieceSize / ByteToMB;
                }
                Pos = Pos + PieceCountInt;

            }
        }

        
    }
}
