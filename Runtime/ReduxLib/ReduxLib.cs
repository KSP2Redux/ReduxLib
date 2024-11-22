using System;
using ReduxLib.Configuration;
using ReduxLib.Logging;
using UnityEngine;
using ILogger = ReduxLib.Logging.ILogger;

namespace ReduxLib;


// ReduxLib itself is a monobehaviour because it needs to be
public class ReduxLib : MonoBehaviour
{
    public static GameObject Instance { get; private set; } = null!;


    private const string LOG_LOCATION_EDITOR = "./Assets/redux.log";
    private const string LOG_LOCATION_PLAYER = "./redux.log";

    private const string CONFIG_LOCATION_EDITOR = "./Assets/redux-config.json";
    private const string CONFIG_LOCATION_PLAYER = "./Redux/config.json";

    private static FileLogProvider _reduxLogProvider;

    internal static ILogger Logger;

    /*
     * The following are going to be used for setting up the global configuration, and setting up loggers
     */

    public static event Action? OnReduxLibInitialized;

    public static IConfigFile ReduxCoreConfig;


    private static ConfigValue<LogLevel> _filterLogLevel;
    private static ConfigValue<bool> _mirrorLogsToUnity;
    private static ConfigValue<string> _logTimestampFormat;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void PreInitializeReduxLib()
    {
        ReduxCoreConfig = new JsonConfigFile(Application.isEditor ? CONFIG_LOCATION_EDITOR : CONFIG_LOCATION_PLAYER);
        _filterLogLevel =
            new(ReduxCoreConfig.Bind("Logging", "Filter Level", LogLevel.Info, "The current log level filter"));
        _mirrorLogsToUnity = new(ReduxCoreConfig.Bind("Logging", "Mirror Logs to Unity", false,
            "Mirror redux logs to unity's debug output?"));
        _logTimestampFormat = new(ReduxCoreConfig.Bind("Logging", "Timestamp Format", "MM/dd/yyyy HH:mm:ss",
            "The timestamp format for logs\n(in C#'s Datetime.ToString format)"));
        _reduxLogProvider = new FileLogProvider(Application.isEditor ? LOG_LOCATION_EDITOR : LOG_LOCATION_PLAYER);
        
        _reduxLogProvider.CurrentFilterLevel = _filterLogLevel.Value;
        _reduxLogProvider.MirrorToUnityLog = _mirrorLogsToUnity.Value;
        _reduxLogProvider.TimestampFormat = _logTimestampFormat.Value;

        _filterLogLevel.RegisterCallback((_, to) => _reduxLogProvider.CurrentFilterLevel = to);
        _mirrorLogsToUnity.RegisterCallback((_, to) => _reduxLogProvider.MirrorToUnityLog = to);
        _logTimestampFormat.RegisterCallback((_, to) => _reduxLogProvider.TimestampFormat = to);
        Logger = _reduxLogProvider.GetLogger("Redux Lib");
        Logger.LogInfo("Redux Lib Pre Initialized!");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeReduxLib()
    {
        Instance = new GameObject("Redux Manager")
        {
            tag = "GameManager",
        };
        DontDestroyOnLoad(Instance);
        Logger.LogInfo("Redux Lib Initialized, calling callbacks!");
        OnReduxLibInitialized?.Invoke();
        Logger.LogInfo("Redux Lib callbacks complete!");
    }


    public void Update()
    {
        _reduxLogProvider.FlushLogs();
    }
}