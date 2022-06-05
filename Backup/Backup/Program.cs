// до 6 версии  пространство имен перед классом объявлялось, сейчас пока так написала, потом найду
// так как параметров входных потенциально может больше, думала как проще сделать маштабируемость работы с ними
//решила сделать отдельным объектом создоваемый файл бекапа,а параметры - это свойства.
//Планирую вынести все в один метод класса BackUP, где аргументы будут все входящие параметры  
//и в main вызывать этот один метод
//TODO: добавить исключение , если параметров нет
BackUpMaker.BackUp fileBackUp = new(args[1]); 

int position = args[0].IndexOf("*."); //определяю переда ли маска в параметр
if (position < 0)
{
    fileBackUp.pathSearch = args[0];
}
else 
{
    fileBackUp.pathSearch = args[0].Substring(0, position);
    fileBackUp.mask = args[0].Substring(position);
}

int paramCount = args.Length; //если их всего 2 , то доп параметров нет
if (paramCount > 2) //перебор массива параметров TODO: вынести в  метод
{ int i = paramCount;
    while (i > 2)
    {
        i--;
        string param = args[i];
        if (param == "--recursive")
        {
            fileBackUp.recursive = true;
        }
        else if (param == "--compress")
        {
            fileBackUp.compress = true;
        }
        else
        {
            fileBackUp.encryptKey = args[i++];
        }
    }
}
EnumerationOptions EnumerationOptions = new EnumerationOptions();
EnumerationOptions.RecurseSubdirectories = fileBackUp.recursive;
string[] files = Directory.GetFiles(fileBackUp.pathSearch, fileBackUp.mask, EnumerationOptions);
//string[] files = GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
//string pathNew = args[1]; //имя файла бекапа

//using (FileStream fsNew = new FileStream(pathNew,
//  FileMode.Create, FileAccess.Write))

FileStream bakfilestream = File.Open(args[1],FileMode.Create);
BinaryWriter bakbinWriter = new BinaryWriter(bakfilestream);
int arFilesLength = files.Length;
bakbinWriter.Write(arFilesLength.ToString());
//переменные для разделения инфромации о записанных данных
//const char nextfile = '>'; //после этого символа идет имя файла
//const char nextfilesize = '<';//после этого символа информация о размере файла

foreach (string file in files)
    {
        try
        {

            using (FileStream fsSource = new FileStream(file,
                FileMode.Open, FileAccess.Read))
            {
                long fileSize = fsSource.Length;
            //пишу имя файла и его размер в байтах,разделяя спец. символами (которые не могут быть в имени файла)
            //выбрала писать строкой т.к. писать например размер файла 
            //метод Write пишет служебную инфу о длине строки перед самой строкой. 
            //Буду использовать метод ReadString при разархивировании, который считает строку ровно той длины, которой записала. 
            //Разделители не нужны.
            bakbinWriter.Write(file + fileSize); 

                // Read the source file into a byte array.
                byte[] bytes = new byte[fsSource.Length];
                int numBytesToRead = (int)fsSource.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    // Read may return anything from 0 to numBytesToRead.
                    int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

                    // Break when the end of the file is reached.
                    if (n == 0)
                        break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                numBytesToRead = bytes.Length;

                // Write the byte array to the other FileStream.    

                {
                    //fsNew.Write(bytes, 0, numBytesToRead);
                    bakbinWriter.Write(bytes, 0, numBytesToRead);


                }
            }
        }
        
        catch (Exception e)
        {
            Console.WriteLine("The process failed: {0}", e.ToString());
        }
   
    }
    bakbinWriter.Close();
    bakfilestream.Close();

        
    




