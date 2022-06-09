using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupV2.Reader
{
    public class FileToRead
    {
        public long FileSize { get; set; }
        public string ShortFileName { get; set; } = string.Empty;
        public string FullFileName { get; set; } = string.Empty;
        public FileToRead()
        {

        }
        public long GetFilePieces(int StandartPieceSize) 
        {
            long pieceCount = FileSize / StandartPieceSize;
                if (pieceCount* StandartPieceSize < FileSize)
                {
                    pieceCount++;
                };
            return pieceCount;
        }

    }
}
