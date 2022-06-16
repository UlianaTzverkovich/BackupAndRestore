using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace Library
{
    public class FileToWrite
    {
        public FileStream StreamForWrite;

        public FileToWrite()
        {
            StreamForWrite = File.Open(GeneralBackupParameters.BackUpFileName, FileMode.Create);
        }
    }
}

