using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupV2.Reader
{
    public class FilesList
    {
        public List<FileToRead>  FileList { get; set; }

        public FilesList(string[] files, string SearchDirectory)
        {
            FileList = new List<FileToRead>();
            foreach (string FileName in files)
            {

                string ShortFileName = FileName.Substring(SearchDirectory.Length);

                FileToRead file1 = new FileToRead();
                file1.ShortFileName = ShortFileName;
                file1.FullFileName = FileName;
                FileInfo FileInfo = new FileInfo(FileName);
                file1.FileSize = FileInfo.Length;


                FileList.Add(file1);
               
            }
        }
    }
 }

