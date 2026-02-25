namespace ReduxLib.Logging;

public class UnityLogger : BaseLogger
{
    private readonly string _name;
    private readonly UnityLogProvider _provider;

    public UnityLogger(string name, UnityLogProvider provider)
    {
        _name = name;
        _provider = provider;
    }

    public override string Name => _name;

    public override void Log(LogLevel level, object x)
    {
        if (level > _provider.CurrentFilterLevel) return;
        UnityEngine.Debug.unityLogger.Log(level.AsLogType(), $"[{_name}] {x}");
        _provider.TriggerLogEvent(this, level, x);
    }
}
