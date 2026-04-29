namespace BertScout2027.Database;

public class BaseDatabase
{
    private string? _databasePath = null;
    public string DatabasePath
    {
        get
        {
            if (_databasePath == null)
            {
#if ANDROID
                _databasePath = "/storage/emulated/0/Documents";
#elif WINDOWS
                if (!Directory.Exists("C:\\Temp"))
                {
                    Directory.CreateDirectory("C:\\Temp");
                }
                _databasePath = "C:\\Temp";
#else
                _databasePath = FileSystem.AppDataDirectory;
#endif
            }
            return _databasePath;
        }
    }

    public BaseDatabase()
    {
    }
}
