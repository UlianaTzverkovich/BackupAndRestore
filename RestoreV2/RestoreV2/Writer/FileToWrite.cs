using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreV2.Writer
{
    public class FileToWrite
    {
        public string[] CatalogArray { get; set; } = new string[0];
        public string ShortFileName { get; set; } =  string.Empty;
        public long FileSize { get; set; } = 0;
        public BinaryWriter? Writer { get; set; } 
        public FileToWrite()
        {

        }
        public void CreateFile(string newFile)
        {            
            FileStream writeStream = File.Open(newFile, FileMode.Create);
            Writer = new BinaryWriter(writeStream);
        }
        public void WriteDataToFile(byte[] data)
        {            
            if (Writer != null) { Writer.Write(data); }
        }
    }
}
