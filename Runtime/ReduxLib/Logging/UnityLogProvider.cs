namespace ReduxLib.Logging;

public class UnityLogProvider : ILogProvider
{
    public LogLevel CurrentFilterLevel { get; set; } = LogLevel.Info;
    public event System.Action<LogLevel, ILogger, object>? OnLog;

    public ILogger GetLogger(string name)
    {
        return new UnityLogger(name, this);
    }

    internal void TriggerLogEvent(ILogger source, LogLevel level, object message)
    {
        OnLog?.Invoke(level, source, message);
    }
}
