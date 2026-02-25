namespace ReduxLib.Logging;

internal class FileLogger : BaseLogger
{
    private readonly string _name;
    private readonly FileLogProvider _provider;

    internal FileLogger(string name, FileLogProvider provider)
    {
        _name = name;
        _provider = provider;
    }

    public override string Name => _name;
    public override void Log(LogLevel level, object x) => _provider.WriteLog(this, level, x);
}