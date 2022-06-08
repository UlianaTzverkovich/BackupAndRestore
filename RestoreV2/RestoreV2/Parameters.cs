using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreV2
{
    public class Parameters
    {
        public string BackUpFileName { get; set; } = string.Empty;
        public string FilesCreateDirectory { get; set; } = string.Empty;
        public int FilesCount { get; set; } = 0;
        public bool Encryption { get; set; } = false;
        public string Password { get; set; } = string.Empty;
        public bool Archive { get; set; } = false;
        public int StandartPieceSize { get; set; } = 64000;
        public int MaxMemoryUse { get; set; } = 500; //макс. объем памяти в мегабайтах
        public int TimeToWait { get; set; } = 500; // время ожидания потоками ресурсов в мс

        string[] Params;
        public Parameters(string[] Params)
        {
            this.Params = Params;
            ParametersProcessing();
        }
        private void ParametersProcessing()
        {
            int paramCount = Params.Length; //если их всего 2 , то доп параметров нет. Если параметров < 2, то прерываем выполнение программы
            if (paramCount < 2)
            {
                Environment.Exit(1);
            }
            else if (paramCount > 2)
            {
                string param = Params[2];

                if (param == "--password" & paramCount>3)
                {
                    Encryption = true;
                    Password = Params[3];
                }
            }
            BackUpFileName = Params[0];
            FilesCreateDirectory = Params[1];
        }
    }
}
