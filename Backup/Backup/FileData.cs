using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backup
{
    public class FileData
    {
        
        //byte[] FileBinaryData;
        int FileSize;
        string ShortFileName;
        FileStream fsSource;
        BinaryWriter BakbinWriter;
        

        public FileData(int FileSize, string ShortFileName, FileStream fsSource, int SumFiles, BinaryWriter BakbinWriter)
        {
            //this.FileBinaryData = FileBinaryData;
            this.FileSize = FileSize;
            this.ShortFileName = ShortFileName;
            this.fsSource = fsSource;
            this.BakbinWriter = new BinaryWriter(fsSource);
            BakbinWriter.Write(SumFiles);
        }

        //public void OpenStreamWrite()
        //{
        //    FileStream BakFileStream = File.Open(pathBack, FileMode.Create);
        //    BakbinWriter = new BinaryWriter(BakFileStream);
        //}

        //public void WriteFileInfo()
        //{            
        //    BakbinWriter.Write(ShortFileName);
        //    BakbinWriter.Write(FileSize.ToString());
        //}

        public void WriteFile()
        {
            byte[] bytes = new byte[fsSource.Length];
            int numBytesToRead = (int)fsSource.Length;
            int numBytesRead = 0;
            //while (numBytesToRead > 0)
            //{
            // Read may return anything from 0 to numBytesToRead.
            //int n =
            fsSource.Read(bytes, numBytesRead, numBytesToRead);
            // Break when the end of the file is reached.
            //  if (n == 0)
            //      break;

            //    numbytesread += n;
            //    numbytestoread -= n;
            //}
            //numBytesToRead = bytes.Length;
            {
                BakbinWriter.Write(bytes, 0, numBytesToRead);
            }

        }
    }
}
