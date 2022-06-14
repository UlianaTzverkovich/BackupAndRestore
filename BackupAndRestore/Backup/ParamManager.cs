using System.Globalization;
using System.Reflection;
using Backup;
using Backup.ThreadsMethods;

namespace Library;

public class ParamManager
{
    /// <summary>
    /// Sets the command line parameters to the class fields.
    /// </summary>
    // public CommandLineParams _commandLineParams;
    public static ServiceInfo ServiceInfo = null!;

    private Dictionary<string, string> _commandLineParams;
    private CommandLineParams commandLineParams;

    public ParamManager(string[] parameters)
    {
        commandLineParams = new CommandLineParams();
        _commandLineParams = commandLineParams.Parameters;
        ServiceInfo = new ServiceInfo();
        ParametersValidation(parameters);
        SetServiceInfo();
    }

    public void ParametersValidation(string[] parameters)
    {
        //Dictionary<string, string?>? dictionaryParams = _commandLineParams.Parameters;
        int paramCount =
            parameters.Length; //если их всего 2 , то доп параметров нет. Если параметров < 2, то прерываем выполнение программы
        if (paramCount < 2)
        {
            Environment.Exit(1);
        }

        int position = parameters[0].IndexOf("*."); //определяю передана ли маска в параметр
        if (position < 0)
        {
            var searchDirectory = parameters[0];
            if (_commandLineParams != null) _commandLineParams.Add("SearchDirectory", searchDirectory);

        }
        else
        {
            var searchDirectory = parameters[0].Substring(0, position);
            var searchMask = parameters[0].Substring(position);
            if (_commandLineParams != null)
            {
                _commandLineParams.Add("FilesDirectory", searchDirectory);
                _commandLineParams.Add("SearchMask", searchMask);
            }
        }

        var backUpFileName = parameters[1];
        if (_commandLineParams != null)
        {
            _commandLineParams.Add("BackUpFileName", backUpFileName);
            //TODO проверка на права и наличие файла

            if (paramCount > 2) //перебор массива параметров 
            {
                int paramsToCount = paramCount;
                var cultureInfo = CultureInfo.InvariantCulture;
                while (paramsToCount > 1)
                {
                    String boolianType = @"#bool";
                    paramsToCount--;
                    string? param = parameters[paramsToCount];
                    if (param == "--recursive")
                    {
                        bool recursive = true;
                        _commandLineParams.Add("Recursive", recursive.ToString(cultureInfo) + boolianType);
                    }
                    else if (param == "--compress")
                    {
                        bool compress = true;
                        _commandLineParams.Add("Compress", compress.ToString(cultureInfo) + boolianType);
                    }
                    else if (param == "--encrypt")
                    {
                        bool encrypt = true;
                        string? password = parameters[paramsToCount + 1];
                        _commandLineParams.Add("Encryption", encrypt.ToString(cultureInfo) + boolianType);
                        _commandLineParams.Add("Password", password);
                    }
                }
            }
        }
    }

    public void SetServiceInfo()
    {
        PropertyInfo[] properties = ServiceInfo.GetType().GetProperties();
        SetParameters(properties);
    }

    public void SetParameters(PropertyInfo[] properties)
    {
        if (properties != null)
        {
            foreach (var currentField in properties)
            {
                if(!_commandLineParams.ContainsKey(currentField.Name)){continue;}
                string currentValue = _commandLineParams[currentField.Name];

                int position = currentValue.IndexOf(@"#");
                if (position > 0)
                {
                    string parameterType = currentValue.Substring(position + 1);
                    string parameterValue = currentValue.Substring(0, position);
                    if (parameterType == "bool")
                    {
                        bool boolParam = Convert.ToBoolean(parameterValue);
                        currentField.SetValue(ServiceInfo, boolParam);
                    }

                }
                else
                {
                    currentField.SetValue(ServiceInfo, currentValue);
                }
            }
        }
    }
}