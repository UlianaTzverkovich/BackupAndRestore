using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupV2.Reader
{
   
    public class FilePieces
    {
        public byte[] PieceData { get; set; } = new byte[0];
        public bool AllowToWrite { get; set; } = false;

        public FilePieces()
        {
           
        }
    }
}
