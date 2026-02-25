using JetBrains.Annotations;

namespace ReduxLib.Logging;

[PublicAPI]
public abstract class BaseLogger : ILogger
{
    public abstract string Name { get; }
    public abstract void Log(LogLevel level, object x);

    public virtual void LogFatal(object x) => Log(LogLevel.Fatal, x);

    public virtual void LogError(object x) => Log(LogLevel.Error, x);

    public virtual void LogWarning(object x) => Log(LogLevel.Warning, x);

    public virtual void LogMessage(object x) => Log(LogLevel.Message, x);

    public virtual void LogInfo(object x) => Log(LogLevel.Info, x);

    public virtual void LogDebug(object x) => Log(LogLevel.Debug, x);
}
