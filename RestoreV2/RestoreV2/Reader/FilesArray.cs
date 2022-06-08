using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestoreV2.Writer;

namespace RestoreV2.Reader
{
    public class FilesArray
    {
        public FileToWrite[] FilesToWrite { get; set; }
        public FilesArray(int filesCount)
        {
            FilesToWrite = new FileToWrite[filesCount];
            Processing(filesCount);
        }
        private void Processing(int filesCount)
        {
            for (int currentFilePos = 0; currentFilePos < filesCount; currentFilePos++)
            {
                FileToWrite currentFile = new FileToWrite();
                FilesToWrite[currentFilePos] = currentFile;
            }
        }
    }
}
