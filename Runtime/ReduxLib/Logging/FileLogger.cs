namespace ReduxLib.Logging;

internal class FileLogger : ILogger
{
    private readonly string _name;
    private readonly FileLogProvider _provider;

    internal FileLogger(string name, FileLogProvider provider)
    {
        _name = name;
        _provider = provider;
    }

    public string Name => _name;
    public void Log(LogLevel level, object x) => _provider.WriteLog(this, level, x);
    
    public void LogFatal(object x) => Log(LogLevel.Fatal, x);

    public void LogError(object x) => Log(LogLevel.Error, x);

    public void LogWarning(object x) => Log(LogLevel.Warning, x);

    public void LogMessage(object x) => Log(LogLevel.Message, x);

    public void LogInfo(object x) => Log(LogLevel.Info, x);

    public void LogDebug(object x) => Log(LogLevel.Debug, x);
}