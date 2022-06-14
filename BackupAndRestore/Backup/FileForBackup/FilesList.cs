using Library;

namespace Backup
{ public class FilesList
    { public List<CurrentFile>? FileList { get; set; }
        public FilesList(string[] Files)
        {
            FileList = new List<CurrentFile>();
            foreach (string FileName in Files)
            {
                if (ParamManager.ServiceInfo != null)
                {
                    string ShortFileName = FileName.Substring(ParamManager.ServiceInfo.FilesDirectory.Length);
                    CurrentFile file1 = new CurrentFile();
                    file1.ShortFileName = ShortFileName;
                    file1.FullFileName = FileName;
                    FileInfo FileInfo = new FileInfo(FileName);
                    file1.FileSize = FileInfo.Length;
                    FileList.Add(file1);
                }
            }
        }
    }
}

