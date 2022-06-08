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
        public void WriteServiceInfo(int NumberOfFiles)
        {
            Writer.Write(NumberOfFiles.ToString());
        }
        public void WriteServiceInfoForFile(BackupV2.Reader.FileToRead file)
        {
            Writer.Write(file.ShortFileName);
            Writer.Write(file.FileSize.ToString());
        }

    }
}
