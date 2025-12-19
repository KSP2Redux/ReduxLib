namespace ReduxLib.GameInterfaces;

public interface ISettingsManager
{
    public static ISettingsManager Instance;

    public void OpenSettingsMenu();
}