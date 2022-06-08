using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUpMaker
{
    public class BackUp //класс объекта создаваемого объекта файла бекапа 
    {
        //пока все с модификатором public, думала над internal , для доступа только в рамках этой сборки
        public string pathSearch { get; set; } = @"с:\";
        public string mask { get; set; } = "*.*";// маска поиска файлов
        public bool recursive { get; set; } = false;// поле значения рекурсивного обхода
        public bool compress { get; set; } = false; // поле значения архивирования
        public string encryptKey // свойство для хранения пароля
        {
            get { return encryptKey; }

            set { encryptKey = value; }
        }
        public int SumFiles { get; set; } // количество файло в бекапе

        public bool WriteStreamOpen { get; set; } = false; //флаг открыт/закрыт стрим на запись

        string pathBack;
        BinaryWriter BakbinWriter;
        public BackUp(string pathBack)
        {
            this.pathBack = pathBack;
        }
        
    }

}
