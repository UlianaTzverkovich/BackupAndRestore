using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Library;

namespace Backup.ThreadsMethods;

public class ProcessingFile
{
    public ProcessingFile() { }
    public void ProcessingFilePiece()
    {
        Encoding u8 = Encoding.UTF8;
            int pos = 0;
            int indexArrayCorrection = 1;
            if (ThreadsManager.FilesList != null)
                foreach (CurrentFile currentFile in ThreadsManager.FilesList)
                {
                    long pieceCount = currentFile.GetFilePieces(SystemRuntimeProp.StandartPieceSize);

                    int pieceCountInt = (int)pieceCount;

                    for (int currentPiecePosition = pos;
                         currentPiecePosition < pieceCountInt + pos;
                         currentPiecePosition++)
                    {
                        while (ThreadsManager.FilePieces != null && currentPiecePosition >
                               ThreadsManager.FilePieces.Count - indexArrayCorrection)
                        {
                            Thread.Sleep(SystemRuntimeProp.TimeToWait);
                        }

                        ; //кусочек несчитался                   
                        if (ThreadsManager.FilePieces != null && ThreadsManager.FilePieces[currentPiecePosition].Processing)
                        {
                            continue;
                        } //данный кусок обрабатывается другим потоком

                        if (ThreadsManager.FilePieces != null)
                        {
                            ThreadsManager.FilePieces[currentPiecePosition].Processing = true;
                            if (ParamManager.ServiceInfo!.Compress | ParamManager.ServiceInfo.Encryption)
                            {
                                if (ParamManager.ServiceInfo.Compress)
                                {
                                    using (var arhivedDataStream = new MemoryStream())
                                    {
                                        using (var compressor =
                                               new GZipStream(arhivedDataStream, CompressionMode.Compress))
                                        using (var clearDataStream =
                                               new MemoryStream(ThreadsManager.FilePieces[currentPiecePosition]
                                                   .PieceData))
                                            clearDataStream.CopyTo(compressor);
                                        ThreadsManager.FilePieces[currentPiecePosition].PieceData =
                                            arhivedDataStream.ToArray();
                                    }
                                }

                                if (ParamManager.ServiceInfo.Encryption)
                                {
                                    using (Aes encryptorAes = Aes.Create())
                                    {
                                        encryptorAes.KeySize = 256;
                                        byte[] bytesPassword = u8.GetBytes(ParamManager.ServiceInfo.Password);
                                        byte[] bytePassword32 = new byte[32];
                                        bytesPassword.CopyTo(bytePassword32, 0);
                                        encryptorAes.Key = bytePassword32;
                                        encryptorAes.IV = new byte[16];
                                        ICryptoTransform encryptor =
                                            encryptorAes.CreateEncryptor(encryptorAes.Key, encryptorAes.IV);
                                        using (MemoryStream encryptDataStream = new MemoryStream())
                                        {
                                            using (CryptoStream csEncrypt = new CryptoStream(encryptDataStream,
                                                       encryptor,
                                                       CryptoStreamMode.Write))
                                            using (var clearDataStream =
                                                   new MemoryStream(ThreadsManager.FilePieces[currentPiecePosition]
                                                       .PieceData))
                                                clearDataStream.CopyTo(csEncrypt);
                                            ThreadsManager.FilePieces[currentPiecePosition].PieceData =
                                                encryptDataStream.ToArray();
                                        }
                                    }
                                }
                            }

                            ThreadsManager.FilePieces[currentPiecePosition].AllowToWrite = true;
                        }
                    }

                    pos = pos + pieceCountInt;
                }
    }
}