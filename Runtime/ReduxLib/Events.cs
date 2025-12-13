using System;

namespace ReduxLib;

public static class Events
{
    public static event Action? MainMenuLoaded;

    public static void TriggerMainMenuLoad()
    {
        MainMenuLoaded?.Invoke();
    }
}