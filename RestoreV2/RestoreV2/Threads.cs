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
        private readonly FileToWrite[] _filesToWrite;
        private readonly List<FilePieces> _filePieces = new List<FilePieces>();
        BackUpFile BackUpFile;
        Parameters Params;
        int StandartPieceSize;
        float MemoryInUse = 0;
        float ByteToMB = 1048576;

        public Threads(FileToWrite[] filesToWrite, Parameters Params, BackUpFile BackUpFile)
        {
            this._filesToWrite = filesToWrite;
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
                currentFile = _filesToWrite[fileCounter];
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

                    while (MemoryInUse >= Params.MaxMemoryUse)
                    {
                        Thread.Sleep(Params.TimeToWait);
                    } //ожидание освобождения памяти

                    _filePieces.Add(piece);
                    MemoryInUse = MemoryInUse + StandartPieceSize / ByteToMB;
                }
            }

            BackUpFile.Reader.Dispose();
        }

        public void ProcessingFilePiece()
        {
            var u8 = Encoding.UTF8;
            int pos = 0;
            int indexArrayCorrection = 1;
            using var compressedDataStream =
                new MemoryStream();
            using var decompressor =
                new GZipStream(compressedDataStream, CompressionMode.Decompress);
            using (Aes encryptorAes = Aes.Create())
                
            {
                encryptorAes.KeySize = 256;
                byte[] bytesPassword = u8.GetBytes(Params.Password);
                byte[] bytePassword32 = new byte[32];
                bytesPassword.CopyTo(bytePassword32, 0);
                encryptorAes.Key = bytePassword32;
                encryptorAes.IV = new byte[16];
                foreach (FileToWrite currentFile in _filesToWrite)
                {
                    for (int currentPiecePos = pos; currentPiecePos < currentFile.PieceCount + pos; currentPiecePos++)
                    {
                        while (currentPiecePos > _filePieces.Count - indexArrayCorrection)
                        {
                            Thread.Sleep(Params.TimeToWait);
                        } //кусочек несчитался 

                        if (_filePieces[currentPiecePos].Processing)
                        {
                            continue;
                        } //кусочек уже в обработке в другом потоке

                        _filePieces[currentPiecePos].Processing = true;
                        if (Params.Archive | Params.Encryption)
                        {
                            if (Params.Encryption)
                            {
                                ICryptoTransform decryptor =
                                    encryptorAes.CreateDecryptor(encryptorAes.Key, encryptorAes.IV);
                                using (MemoryStream decryptDataStream =
                                       new MemoryStream(_filePieces[currentPiecePos].PieceData))
                                {
                                    using (CryptoStream csDecrypt = new CryptoStream(decryptDataStream, decryptor,
                                               CryptoStreamMode.Read))
                                    using (var clearDataStream = new MemoryStream())
                                    {
                                        csDecrypt.CopyTo(clearDataStream);
                                        _filePieces[currentPiecePos].PieceData = clearDataStream.ToArray();
                                    }
                                }
                            }

                            if (Params.Archive)
                            {
                                compressedDataStream.SetLength(0);
                                compressedDataStream.Write(_filePieces[currentPiecePos].PieceData, 0, _filePieces[currentPiecePos].PieceData.Length);
                                //Console.WriteLine(compressedDataStream.Length);
                                //_filePieces[currentPiecePos].PieceData.CopyTo(compressedDataStream);
                                //compressedDataStream.ReadByte(_filePieces[currentPiecePos].PieceData);
                                 //   new MemoryStream(_filePieces[currentPiecePos].PieceData);
                                /*using var decompressor =
                                    new GZipStream(compressedDataStream, CompressionMode.Decompress);*/
                                using var clearDataStream = new MemoryStream();
                                decompressor.CopyTo(clearDataStream);
                                _filePieces[currentPiecePos].PieceData = clearDataStream.ToArray();
                            }
                        }

                        _filePieces[currentPiecePos].AllowToWrite = true;
                    }

                    pos = pos + currentFile.PieceCount;
                }
            }
        }


        public void WriteToFile()
        {
            int pos = 0;
            int indexArrayCorrection = 1;
            foreach (FileToWrite currentFile in _filesToWrite)
            {
                while (currentFile.FileSize == 0)
                {
                    Thread.Sleep(Params.TimeToWait);
                }

                ; //ожидание заполения массива с файлами
                currentFile.CreateFile(Params.FilesCreateDirectory + currentFile.ShortFileName);
                for (int currentPiecePos = pos; currentPiecePos < currentFile.PieceCount + pos; currentPiecePos++)
                {
                    while (currentPiecePos > _filePieces.Count - indexArrayCorrection)
                    {
                        Thread.Sleep(Params.TimeToWait);
                    } //кусочек несчитался 

                    while (!_filePieces[currentPiecePos].AllowToWrite)
                    {
                        Thread.Sleep(Params.TimeToWait);
                    } //кусочек недобработался 

                    if (currentFile.Writer != null)
                    {
                        currentFile.Writer.Write(_filePieces[currentPiecePos].PieceData);
                    }

                    float pieceSize = _filePieces[currentPiecePos].PieceData.Length;
                    _filePieces[currentPiecePos].PieceData = new byte[0]; // освобождение памяти?
                    MemoryInUse = MemoryInUse - StandartPieceSize / ByteToMB;
                }

                if (currentFile.Writer != null)
                {
                    currentFile.Writer.Dispose();
                }

                pos = pos + currentFile.PieceCount;
            }
        }
    }
}