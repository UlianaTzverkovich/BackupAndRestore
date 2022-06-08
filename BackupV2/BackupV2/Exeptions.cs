using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupV2
{
    public class Exeptions //класс доп обработки ошибок
    {

        public Exeptions(Exception error)
        { 
        Console.WriteLine("The process failed: {0}", error.ToString());
        }

    }
}
