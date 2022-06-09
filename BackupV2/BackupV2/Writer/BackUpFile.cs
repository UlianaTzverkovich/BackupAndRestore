using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupV2.Writer
{
    public class BackUpFile 
    {
        public BinaryWriter Writer { get; set; }

        public BackUpFile(string pathBack)
        {
            FileStream BakFileStream = File.Open(pathBack, FileMode.Create);
            Writer = new BinaryWriter(BakFileStream);
        }
        public void WriteServiceInfo(int NumberOfFiles, Parameters parameters)
        {
            Writer.Write(NumberOfFiles.ToString());
            Writer.Write(parameters.Archive);
            Writer.Write(parameters.StandartPieceSize);
            Writer.Write(parameters.Encryption);
        }
        public void WriteServiceInfoForFile(Reader.FileToRead file, int pieceCount)
        {
            Writer.Write(file.ShortFileName);
            Writer.Write(file.FileSize.ToString());
            Writer.Write(pieceCount);
        }
        public void WritePieceOfData(byte[] data)
        {
           Writer.Write(data.Length.ToString());            
           Writer.Write(data);
        }

    }
}
