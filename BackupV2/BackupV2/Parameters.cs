using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupV2
{
    public class Parameters
    {
        public string SearchDirectory { get; set; } = string.Empty;
        public string SearchMask { get; set; } = string.Empty; 
        public string BackUpFileName { get; set; } = string.Empty;
        public bool Recursive { get; set; } = false;
        public bool Archive { get; set; } = false;
        public bool Encryption { get; set; } = false;
        public string Password { get; set; } = string.Empty;
        public int StandartPieceSize { get; set; } = 64000;
        public int MaxMemoryUse{ get; set; } = 500; //макс. объем памяти в мегабайтах
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
            int position = Params[0].IndexOf("*."); //определяю передана ли маска в параметр
            if (position < 0)
            {
                SearchDirectory = Params[0];
            }
            else
            {
                SearchDirectory = Params[0].Substring(0, position);
                SearchMask = Params[0].Substring(position);
            }

            BackUpFileName= Params[1];

            if (paramCount > 2) //перебор массива параметров TODO: вынести в  метод
            {
                int paramsToCount = paramCount;
                while (paramsToCount > 1)
                {
                    paramsToCount--;
                    string param = Params[paramsToCount];
                    if (param == "--recursive")
                    {
                        Recursive = true;
                    }
                    else if (param == "--compress")
                    {
                        Archive = true;
                    }
                    else if (param == "--encrypt")
                    {
                        Encryption = true;

                        Password = Params[paramsToCount++];
                    }
                }
            }

        }

    }
}
