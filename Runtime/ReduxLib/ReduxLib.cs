using System;
using ReduxLib.Configuration;
using ReduxLib.Logging;
using UnityEngine;
using UnityEngine.UIElements;
using ILogger = ReduxLib.Logging.ILogger;

namespace ReduxLib;


// ReduxLib itself is a monobehaviour because it needs to be
public class ReduxLib : MonoBehaviour
{
    public static ReduxLib Instance { get; private set; } = null!;


    public const string REDUX_FOLDER = "Redux";

    private const string LOG_LOCATION = "./redux.log";

    private const string CONFIG_LOCATION = "./Redux/config.json";

    public static FileLogProvider ReduxLogProvider;

    internal static ILogger Logger;

    /*
     * The following are going to be used for setting up the global configuration, and setting up loggers
     */

    public static event Action? OnReduxLibInitialized;

    public static IConfigFile ReduxCoreConfig;

    private static ConfigValue<LogLevel> _filterLogLevel;
    private static ConfigValue<bool> _mirrorLogsToUnity;
    private static ConfigValue<string> _logTimestampFormat;
    private static ConfigValue<bool> _usePhysicsAutosync;
    private static ConfigValue<bool> _inputDamping;

    private static ConfigValue<float> _inputDampingSensitivityDefault;
    private static ConfigValue<float> _inputDampingSensitivityPrecise;
    private static ConfigValue<float> _inputDampingReturnSpeed;
    public static bool InputDamping => _inputDamping.Value;
    public static float InputDampingSensitivityDefault => _inputDampingSensitivityDefault.Value;
    public static float InputDampingSensitivityPrecise => _inputDampingSensitivityPrecise.Value;
    public static float InputDampingReturnSpeed => _inputDampingReturnSpeed.Value;
    

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void PreInitializeReduxLib()
    {
        ReduxCoreConfig = new JsonConfigFile(CONFIG_LOCATION);
        _filterLogLevel =
            new(ReduxCoreConfig.Bind("Logging", "Filter Level", LogLevel.Info, "The current log level filter"));
        _mirrorLogsToUnity = new(ReduxCoreConfig.Bind("Logging", "Mirror Logs to Unity", false,
            "Mirror redux logs to unity's debug output?"));
        _logTimestampFormat = new(ReduxCoreConfig.Bind("Logging", "Timestamp Format", "MM/dd/yyyy HH:mm:ss",
            "The timestamp format for logs\n(in C#'s Datetime.ToString format)"));
        _usePhysicsAutosync = new(ReduxCoreConfig.Bind("Advanced", "Use Unity physics auto sync", false,
            "Enable Unity's Physics.autoSyncTransforms (slower) option for troubleshooting purposes. Please file a bug report if enabling this fixes a physics issue."));
        _inputDamping = new(ReduxCoreConfig.Bind("Input", "Input Damping", true,
            "Enable input damping for pitch/yaw/roll/steer controls"));
        _inputDampingSensitivityDefault = new(ReduxCoreConfig.Bind("Input",
            "Input Damping Sensitivity", 10f,
            "The input damping sensitivity when not in precision control mode\nIs the inverse of how many seconds it takes to go from 0 to 100% authority",
            new ListConstraint<float>(2f, 20f)));
        _inputDampingSensitivityPrecise = new(ReduxCoreConfig.Bind("Input", "Input Damping Sensitivity (Precise)", 3f,
            "The input damping sensitivity when in precision control mode\nIs the inverse of how many seconds it takes to go from 0 to 100% authority",new ListConstraint<float>(0.5f, 10f)));
        _inputDampingReturnSpeed = new(ReduxCoreConfig.Bind("Input", "Input Damping Return Speed", 8f,
            "The input damping return speed\nInverse of how many seconds it takes to reset to 0% authority from 100%",
            new ListConstraint<float>(6f, 30f)));
        
        
        ReduxLogProvider = new FileLogProvider(LOG_LOCATION)
            {
                CurrentFilterLevel = _filterLogLevel.Value,
                MirrorToUnityLog = _mirrorLogsToUnity.Value,
                TimestampFormat = _logTimestampFormat.Value
            };
        Physics.autoSyncTransforms = _usePhysicsAutosync.Value;

        _filterLogLevel.RegisterCallback((_, to) => ReduxLogProvider.CurrentFilterLevel = to);
        _mirrorLogsToUnity.RegisterCallback((_, to) => ReduxLogProvider.MirrorToUnityLog = to);
        _logTimestampFormat.RegisterCallback((_, to) => ReduxLogProvider.TimestampFormat = to);
        _usePhysicsAutosync.RegisterCallback((_, to) => Physics.autoSyncTransforms = to);
        Logger = ReduxLogProvider.GetLogger("Redux Lib");
        Logger.LogInfo("Redux Lib Pre Initialized!");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeReduxLib()
    {
        var instance = new GameObject("Redux Manager")
        {
            tag = "Game Manager",
        };
        Instance = instance.AddComponent<ReduxLib>();
        DontDestroyOnLoad(Instance);
        Logger.LogInfo("Redux Lib Initialized, calling callbacks!");
        if (OnReduxLibInitialized != null)
        {
            // Do this manually so even if an error happens, later ones aren't affected
            foreach (var del in OnReduxLibInitialized.GetInvocationList())
            {
                try
                {
                    del.Method.Invoke(del.Target, new object[] { });
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }
        Logger.LogInfo("Redux Lib callbacks complete!");
    }

    public static ILogger GetLogger(string name)
    {
        return ReduxLogProvider.GetLogger(name);
    }

    public static GameObject GetAlwaysLoadedObject(string name)
    {
        var result = new GameObject(name)
        {
            tag = "Game Manager"
        };
        result.transform.SetParent(Instance.transform);
        return result;
    }

    public void Update()
    {
        ReduxLogProvider.FlushLogs();
    }
}