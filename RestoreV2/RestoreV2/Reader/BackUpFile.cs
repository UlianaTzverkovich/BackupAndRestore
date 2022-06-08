using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestoreV2.Writer;

namespace RestoreV2.Reader
{
    public class BackUpFile
    {  
        public BinaryReader Reader { get; set; }
        public BackUpFile(string pathBack)
        {
            FileStream bakFileStream = File.Open(pathBack, FileMode.Open);
            Reader = new BinaryReader(bakFileStream);
        }
        public string ReadServiceInfo()
        {
            string serviceInfo = Reader.ReadString();

            //добавить обработку других параметров бекапа (архивация, шифрование)
            return serviceInfo;
        }
        public byte[] ReadFileContent(int StandartPieceSize)
        {
            return Reader.ReadBytes(StandartPieceSize);
        }
    }
}
