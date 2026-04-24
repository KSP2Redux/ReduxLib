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
    public const string REDUX_FOLDER = "Redux";
    private const string CONFIG_LOCATION = "./Redux/config.json";

    private const string LOGGING_CONFIG_SECTION = "Logging";
    private const string FLIGHT_INPUT_CONFIG_SECTION = "Flight Input";
    private const string ADVANCED_CONFIG_SECTION = "Advanced";

    public static ReduxLib Instance { get; private set; } = null!;
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

    private static ConfigValue<float> _flightInputNormalSensitivity;
    private static ConfigValue<float> _flightInputNormalGravity;
    private static ConfigValue<float> _flightInputPrecisionRate;
    private static ConfigValue<float> _flightInputPrecisionGravity;

    public static string TimestampFormat => _logTimestampFormat.Value;

    public static float FlightInputNormalSensitivity => _flightInputNormalSensitivity.Value;
    public static float FlightInputNormalGravity => _flightInputNormalGravity.Value;
    public static float FlightInputPrecisionRate => _flightInputPrecisionRate.Value;
    public static float FlightInputPrecisionGravity => _flightInputPrecisionGravity.Value;


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
                    LOGGING_CONFIG_SECTION,
                    "Filter Level",
                    LogLevel.Info,
                    "The current log level filter"
                )
            );
        _logTimestampFormat = new ConfigValue<string>(
            ReduxCoreConfig.Bind(
                LOGGING_CONFIG_SECTION,
                "Timestamp Format",
                "HH:mm:ss.fff",
                "The timestamp format for logs\n(in C#'s Datetime.ToString format)"
            )
        );
        _usePhysicsAutosync = new ConfigValue<bool>(
            ReduxCoreConfig.Bind(
                ADVANCED_CONFIG_SECTION,
                "Use Unity physics auto sync",
                false,
                "Enable Unity's Physics.autoSyncTransforms (slower) option for troubleshooting purposes. Please file a bug report if enabling this fixes a physics issue."
            )
        );
        _flightInputNormalSensitivity = new ConfigValue<float>(
            ReduxCoreConfig.Bind(
                FLIGHT_INPUT_CONFIG_SECTION,
                "Normal Sensitivity",
                8f,
                "Units per second to ramp keyboard pitch, yaw, and roll input toward +/-1 while held.",
                new RangeConstraint<float>(0f, 20f, 400)
            )
        );
        _flightInputNormalGravity = new ConfigValue<float>(
            ReduxCoreConfig.Bind(
                FLIGHT_INPUT_CONFIG_SECTION,
                "Normal Decay",
                8f,
                "Units per second to return keyboard pitch, yaw, and roll input toward 0 after release.",
                new RangeConstraint<float>(0f, 20f, 400)
            )
        );
        _flightInputPrecisionRate = new ConfigValue<float>(
            ReduxCoreConfig.Bind(
                FLIGHT_INPUT_CONFIG_SECTION,
                "Precision Rate",
                0.4f,
                "Units per second to accumulate pitch, yaw, and roll input while precision mode keys are held.",
                new RangeConstraint<float>(0f, 5f, 500)
            )
        );
        _flightInputPrecisionGravity = new ConfigValue<float>(
            ReduxCoreConfig.Bind(
                FLIGHT_INPUT_CONFIG_SECTION,
                "Precision Decay",
                1f,
                "Units per second to return accumulated precision mode pitch, yaw, and roll input toward 0.",
                new RangeConstraint<float>(0f, 5f, 500)
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
