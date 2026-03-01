using System;
using System.IO;
using ReduxLib.Configuration;
using ReduxLib.Logging;
using UnityEngine;
using ILogger = ReduxLib.Logging.ILogger;

namespace ReduxLib;

// ReduxLib itself is a monobehaviour because it needs to be
public class ReduxLib : MonoBehaviour
{
    public static ReduxLib Instance { get; private set; } = null!;


    public const string REDUX_FOLDER = "Redux";

    private const string CONFIG_LOCATION = "./Redux/config.json";

    public static ILogProvider ReduxLogProvider;

    internal static ILogger Logger;

    /*
     * The following are going to be used for setting up the global configuration and setting up loggers
     */

    public static event Action? OnReduxLibInitialized;

    public static IConfigFile ReduxCoreConfig;

    private static ConfigValue<LogLevel> _filterLogLevel;
    private static ConfigValue<string> _logTimestampFormat;

    private static ConfigValue<bool> _usePhysicsAutosync;

    private static ConfigValue<bool> _inputDamping;
    private static ConfigValue<float> _inputDampingSensitivityDefault;
    private static ConfigValue<float> _inputDampingSensitivityPrecise;
    private static ConfigValue<float> _inputDampingReturnSpeed;

    public static string TimestampFormat => _logTimestampFormat.Value;

    public static bool InputDamping => _inputDamping.Value;
    public static float InputDampingSensitivityDefault => _inputDampingSensitivityDefault.Value;
    public static float InputDampingSensitivityPrecise => _inputDampingSensitivityPrecise.Value;
    public static float InputDampingReturnSpeed => _inputDampingReturnSpeed.Value;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void PreInitializeReduxLib()
    {
        if (!Directory.Exists("./Redux/Config"))
        {
            Directory.CreateDirectory("./Redux/Config");
        }
        ReduxCoreConfig = new JsonConfigFile(CONFIG_LOCATION);
        _filterLogLevel =
            new ConfigValue<LogLevel>(
                ReduxCoreConfig.Bind(
                    "Logging",
                    "Filter Level",
                    LogLevel.Info,
                    "The current log level filter"
                )
            );
        _logTimestampFormat = new ConfigValue<string>(
            ReduxCoreConfig.Bind(
                "Logging",
                "Timestamp Format",
                "HH:mm:ss.fff",
                "The timestamp format for logs\n(in C#'s Datetime.ToString format)"
            )
        );
        _usePhysicsAutosync = new ConfigValue<bool>(
            ReduxCoreConfig.Bind(
                "Advanced",
                "Use Unity physics auto sync",
                false,
                "Enable Unity's Physics.autoSyncTransforms (slower) option for troubleshooting purposes. Please file a bug report if enabling this fixes a physics issue."
            )
        );


        ReduxLogProvider = new UnityLogProvider
        {
            CurrentFilterLevel = _filterLogLevel.Value,
        };
        Physics.autoSyncTransforms = _usePhysicsAutosync.Value;

        _filterLogLevel.RegisterCallback((_, to) => ReduxLogProvider.CurrentFilterLevel = to);
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
            foreach (Delegate? del in OnReduxLibInitialized.GetInvocationList())
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
        if (ReduxLogProvider is IUpdatableLogProvider updatable)
        {
            updatable.Update();
        }
    }
}