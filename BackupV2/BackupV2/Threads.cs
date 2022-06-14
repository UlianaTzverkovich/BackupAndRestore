using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using BackupV2.Reader;
using System.Security.Cryptography;

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
            foreach (FileToRead currentFile in FileList)
            {
                long pieceCount = currentFile.GetFilePieces(StandartPieceSize);
                long lastPieceSize = currentFile.FileSize - (pieceCount - 1) * StandartPieceSize;
                FileStream filestream = File.Open(currentFile.FullFileName, FileMode.Open); 
                BinaryReader fileReader = new BinaryReader(filestream);

                for (int currentPiecePosition = 1; currentPiecePosition <= pieceCount; currentPiecePosition++)
                {
                    FilePieces piece = new FilePieces();
                    if (currentPiecePosition == pieceCount) { pieceToRead = (int)lastPieceSize; }
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
        public void ProcessingFilePiece()
        {
            Encoding u8 = Encoding.UTF8;
            int pos = 0;
            int indexArrayCorrection = 1;
            foreach (FileToRead currentFile in FileList)
            {
                long pieceCount = currentFile.GetFilePieces(StandartPieceSize);
             
                int pieceCountInt = (int)pieceCount;

                for (int currentPiecePosition = pos; currentPiecePosition < pieceCountInt + pos; currentPiecePosition++)
                {
                    while (currentPiecePosition > FilePieces.Count - indexArrayCorrection) { Thread.Sleep(Params.TimeToWait); }; //кусочек несчитался                   
                    if (FilePieces[currentPiecePosition].Processing) { continue; } //данный кусок обрабатывается другим потоком
                    FilePieces[currentPiecePosition].Processing = true;
                    if (Params.Archive | Params.Encryption)
                    {
                        if (Params.Archive)
                        {
                            using (var arhivedDataStream = new MemoryStream())
                            {
                                using (var compressor = new GZipStream(arhivedDataStream, CompressionMode.Compress))
                                using (var clearDataStream = new MemoryStream(FilePieces[currentPiecePosition].PieceData))
                                    clearDataStream.CopyTo(compressor);
                                FilePieces[currentPiecePosition].PieceData = arhivedDataStream.ToArray();
                            }
                        }
                        if (Params.Encryption)
                        {
                            using (Aes encryptorAes = Aes.Create())
                            {
                                encryptorAes.KeySize = 256;
                                byte[] bytesPassword = u8.GetBytes(Params.Password);
                                byte[] bytePassword32 = new byte[32];
                                bytesPassword.CopyTo(bytePassword32, 0);
                                encryptorAes.Key = bytePassword32;
                                encryptorAes.IV = new byte[16];
                                ICryptoTransform encryptor = encryptorAes.CreateEncryptor(encryptorAes.Key, encryptorAes.IV);
                                using (MemoryStream encryptDataStream = new MemoryStream())
                                {
                                    using (CryptoStream csEncrypt = new CryptoStream(encryptDataStream, encryptor, CryptoStreamMode.Write))
                                    using (var clearDataStream = new MemoryStream(FilePieces[currentPiecePosition].PieceData))
                                        clearDataStream.CopyTo(csEncrypt);
                                    FilePieces[currentPiecePosition].PieceData = encryptDataStream.ToArray();
                                }
                            }
                        }                        
                    }

                    FilePieces[currentPiecePosition].AllowToWrite = true;
                }
                pos = pos + pieceCountInt;
            }
        }

        public void WriteToFile()
        {
            int pos = 0;
            int indexArrayCorrection = 1;
            foreach (FileToRead currentFile in FileList)
            {
                long pieceCount = currentFile.GetFilePieces(StandartPieceSize);
                int pieceCountInt = (int)pieceCount;
                BackUpFile.WriteServiceInfoForFile(currentFile, pieceCountInt);

                for (int currentPiecePosition = pos; currentPiecePosition < pieceCountInt + pos; currentPiecePosition++)
                {
                    while (currentPiecePosition > FilePieces.Count - indexArrayCorrection) { Thread.Sleep(Params.TimeToWait); }; //кусочек несчитался 
                    while (!FilePieces[currentPiecePosition].AllowToWrite) { Thread.Sleep(Params.TimeToWait); }; //кусочек недобработался                                        
                    BackUpFile.WritePieceOfData(FilePieces[currentPiecePosition].PieceData);
                    float PieceSize = FilePieces[currentPiecePosition].PieceData.Length;
                    FilePieces[currentPiecePosition].PieceData = new byte[0]; // освобождение памяти
                    MemoryInUse = MemoryInUse - PieceSize / ByteToMB;
                }
                pos = pos + pieceCountInt;

            }
        }

    }
}
