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
        public void ReadServiceInfo(Parameters parameters)
        {
            parameters.FilesCount = Convert.ToInt32(Reader.ReadString());
            parameters.Archive = Reader.ReadBoolean();
            parameters.StandartPieceSize = Reader.ReadInt32();
            parameters.Encryption = Reader.ReadBoolean();
            if (parameters.Password == string.Empty & parameters.Encryption)
            {
                throw new ArgumentNullException("Файл зашифрован. Пароль не указан.");
            }
         }
        public string ReadServiceInfoString()
        {
            string serviceInfo = Reader.ReadString();
            return serviceInfo;
        }
        public byte[] ReadFileContent(int StandartPieceSize)
        {       
            byte[] data = Reader.ReadBytes(StandartPieceSize);
            return data;
        }

        public int ReadServiceInfoInt()
        {
            int serviceInfo = Reader.ReadInt32();
            return serviceInfo;
        }
    }
}
