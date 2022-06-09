using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestoreV2.Writer;
using RestoreV2.Reader;
using System.Security.Cryptography;
using System.IO.Compression;
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
                currentFile.ShortFileName = BackUpFile.ReadServiceInfoString();
                currentFile.CreateDirectories(Params.FilesCreateDirectory);
                currentFile.FileSize = Convert.ToInt64(BackUpFile.ReadServiceInfoString());
                currentFile.PieceCount = BackUpFile.ReadServiceInfoInt();
                for (int currentPiece = 1; currentPiece <= currentFile.PieceCount; currentPiece++)
                {
                    FilePieces piece = new FilePieces();
                    pieceToRead = Convert.ToInt32(BackUpFile.ReadServiceInfoString());
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

            BackUpFile.Reader.Dispose();

        }

        public void ProcessingFilePiece()
        {
            
            Encoding u8 = Encoding.UTF8;
            int pos = 0;
            int indexArrayCorrection = 1;
            foreach (FileToWrite currentFile in FilesToWrite)
            {
                for (int currentPiecePos = pos; currentPiecePos < currentFile.PieceCount + pos; currentPiecePos++)
                {
                    while (currentPiecePos > FilePieces.Count - indexArrayCorrection) { Thread.Sleep(Params.TimeToWait); } //кусочек несчитался 
                    if (FilePieces[currentPiecePos].Processing) { continue; } //кусочек уже в обработке в другом потоке
                    FilePieces[currentPiecePos].Processing = true;
                    if (Params.Archive | Params.Encryption)
                    {
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
                                ICryptoTransform decryptor = encryptorAes.CreateDecryptor(encryptorAes.Key, encryptorAes.IV);
                                using (MemoryStream decryptDataStream = new MemoryStream(FilePieces[currentPiecePos].PieceData))
                                {
                                    using (CryptoStream csDecrypt = new CryptoStream(decryptDataStream, decryptor, CryptoStreamMode.Read))
                                    using (var clearDataStream = new MemoryStream())
                                    {
                                        csDecrypt.CopyTo(clearDataStream);
                                        FilePieces[currentPiecePos].PieceData = clearDataStream.ToArray();
                                    }
                                }
                            }
                        }
                        if (Params.Archive)
                        {
                            using (var compressedDataStream = new MemoryStream(FilePieces[currentPiecePos].PieceData))
                                
                            using (var decompressor = new GZipStream(compressedDataStream, CompressionMode.Decompress))
                            using (var clearDataStream = new MemoryStream())
                            {
                               decompressor.CopyTo(clearDataStream);
                               FilePieces[currentPiecePos].PieceData = clearDataStream.ToArray();                                
                            }

                        }

                    }
                    FilePieces[currentPiecePos].AllowToWrite = true;
                }
                pos = pos + currentFile.PieceCount;
            }

        }


        public void WriteToFile()
        {
            int pos = 0;
            int indexArrayCorrection = 1;
            foreach (FileToWrite currentFile in FilesToWrite)
            {
                while (currentFile.FileSize == 0) { Thread.Sleep(Params.TimeToWait); }; //ожидание заполения массива с файлами
                currentFile.CreateFile(Params.FilesCreateDirectory + currentFile.ShortFileName);
                for (int currentPiecePos = pos; currentPiecePos < currentFile.PieceCount + pos; currentPiecePos++)
                {
                    while (currentPiecePos > FilePieces.Count - indexArrayCorrection) { Thread.Sleep(Params.TimeToWait); } //кусочек несчитался 
                    while (!FilePieces[currentPiecePos].AllowToWrite) { Thread.Sleep(Params.TimeToWait); } //кусочек недобработался 
                    if (currentFile.Writer != null) { currentFile.Writer.Write(FilePieces[currentPiecePos].PieceData); }
                    float pieceSize = FilePieces[currentPiecePos].PieceData.Length;
                    FilePieces[currentPiecePos].PieceData = new byte[0]; // освобождение памяти?
                    MemoryInUse = MemoryInUse - pieceSize / ByteToMB;
                }
                if (currentFile.Writer != null) { currentFile.Writer.Dispose(); }

                pos = pos + currentFile.PieceCount;
            }
        }

    }
}
