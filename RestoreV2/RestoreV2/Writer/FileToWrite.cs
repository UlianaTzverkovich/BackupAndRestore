using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreV2.Writer
{
    public class FileToWrite
    {
        public string[] CatalogArray { get; set; } = new string[0];
        public string ShortFileName { get; set; } =  string.Empty;
        public long FileSize { get; set; } = 0;
        public int PieceCount { get; set; } = 0;
        public BinaryWriter? Writer { get; set; } 
        public FileToWrite()
        {

        }
        public void CreateFile(string newFile)
        {            
            FileStream writeStream = File.Open(newFile, FileMode.Create);
            Writer = new BinaryWriter(writeStream);
        }
        public void WriteDataToFile(byte[] data)
        {            
            if (Writer != null) { Writer.Write(data); }
        }

        public void CreateDirectories(string FilesCreateDirectory)
        {
            string currentDirectory = FilesCreateDirectory;
            string pathSpliter = @"\";
            string[] DirectoriesArray = ShortFileName.Split(pathSpliter);
            int arrayLength = DirectoriesArray.Length;            
            if (DirectoriesArray.Length == 1) { return; }
            for (int dirpos = 0; dirpos < arrayLength - 1; dirpos++)
            {  if (currentDirectory.Substring(currentDirectory.Length - 1) != pathSpliter)
                {                   
                    currentDirectory = currentDirectory + pathSpliter ;
                }
                currentDirectory = currentDirectory + DirectoriesArray[dirpos];
                if (!Directory.Exists(currentDirectory)) { Directory.CreateDirectory(currentDirectory); }               
            }

        }
    }
}
