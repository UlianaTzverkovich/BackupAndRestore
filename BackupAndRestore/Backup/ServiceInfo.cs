using System.Reflection;
using Library;

namespace Backup;
public class ServiceInfo : GeneralBackupParameters
{
    public string SearchMask { get; set; } = string.Empty;
    public ServiceInfo() { }
}