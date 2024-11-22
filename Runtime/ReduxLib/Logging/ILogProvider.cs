namespace ReduxLib.Logging;

public interface ILogProvider
{
    public ILogger GetLogger(string name);
}