using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace Backup
{
    public class BackUpFile
    {
        public BinaryWriter Writer { get; } = BinaryWriter.Null;
        public BackUpFile()
        {
            if (ParamManager.ServiceInfo != null)
            {
                if (ParamManager.ServiceInfo?.BackUpFileName != null)
                {
                    FileStream bakFileStream = File.Open(ParamManager.ServiceInfo.BackUpFileName, FileMode.Create);
                    Writer = new BinaryWriter(bakFileStream);
                }
            }
        }
        public void WriteServiceInfo()
        {
            if (ParamManager.ServiceInfo != null)
            {
                Writer.Write(ParamManager.ServiceInfo.FilesCount.ToString());
                Writer.Write(ParamManager.ServiceInfo.Compress);
                Writer.Write(SystemRuntimeProp.StandartPieceSize);
                Writer.Write(ParamManager.ServiceInfo.Encryption);
            }
        }
        public void WriteServiceInfoForFile(CurrentFile file, int pieceCount)
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
