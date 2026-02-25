namespace ReduxLib.Logging;

public interface ILogProvider
{
    public LogLevel CurrentFilterLevel { get; set; }
    public event System.Action<LogLevel, ILogger, object>? OnLog;
    public ILogger GetLogger(string name);
}

public interface IUpdatableLogProvider
{
    public void Update();
}