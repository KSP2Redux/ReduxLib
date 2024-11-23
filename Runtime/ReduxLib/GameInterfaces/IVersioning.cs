namespace ReduxLib.GameInterfaces;

public interface IVersioning
{
    public static IVersioning Instance;
    public string Version { get; }
}