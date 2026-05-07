using System;
using System.IO;
using ReduxLib.Configuration;
using ReduxLib.Configuration.Attributes;
using ReduxLib.Logging;
using UnityEngine;
using ILogger = ReduxLib.Logging.ILogger;

namespace ReduxLib;

internal class ReduxLibConfig
{
    [ConfigSection("Logging", loc: "Menu/Settings/Sections/Logging")]
    [ConfigValue("Filter Level",
        "The current log level filter",
        nameLoc: "Menu/Settings/FilterLevel",
        descLoc: "Menu/Settings/Description/FilterLevel")]
    public ConfigValue<LogLevel> FilterLogLevel = new ConfigDescription<LogLevel>(LogLevel.Info);

    [ConfigValue("Timestamp Format",
        "The timestamp format for logs\n(in C#'s Datetime.ToString format)",
        nameLoc: "Menu/Settings/LogTimestampFormat",
        descLoc: "Menu/Settings/Description/LogTimestampFormat")]
    public string LogTimestampFormat = "HH:mm:ss.fff";

    [ConfigSection("Advanced", loc: "Menu/Settings/Sections/Advanced")]
    [ConfigValue("Use Unity physics auto sync",
        "Enable Unity's Physics.autoSyncTransforms (slower) option for troubleshooting purposes. Please file a bug report if enabling this fixes a physics issue.",
        nameLoc: "Menu/Settings/UseUnityPhysicsAutoSync",
        descLoc: "Menu/Settings/Description/UseUnityPhysicsAutoSync")]
    public ConfigValue<bool> UsePhysicsAutosync = new ConfigDescription<bool>(false);

    [ConfigSection("Startup", loc: "Menu/Settings/Sections/Startup")]
    [ConfigValue("Disable photosensitivity warning",
        "Skips the startup photosensitivity warning.",
        nameLoc: "Menu/Settings/DisablePhotosensitivityWarning",
        descLoc: "Menu/Settings/Description/DisablePhotosensitivityWarning")]
    public bool DisablePhotosensitivityWarning;

    [ConfigSection("Flight Input", loc: "Menu/Settings/Sections/FlightInput")]
    [ConfigValue("Normal Sensitivity",
        "Units per second to ramp keyboard pitch, yaw, and roll input toward +/-1 while held.",
        nameLoc: "Menu/Settings/NormalSensitivity",
        descLoc: "Menu/Settings/Description/NormalSensitivity")]
    [ConfigRange(0f, 20f, 400)]
    public float FlightInputNormalSensitivity = 8f;

    [ConfigValue("Normal Decay",
        "Units per second to return keyboard pitch, yaw, and roll input toward 0 after release.",
        nameLoc: "Menu/Settings/NormalDecay",
        descLoc: "Menu/Settings/Description/NormalDecay")]
    [ConfigRange(0f, 20f, 400)]
    public float FlightInputNormalGravity = 8f;

    [ConfigValue("Precision Rate",
        "Units per second to accumulate pitch, yaw, and roll input while precision mode keys are held.",
        nameLoc: "Menu/Settings/PrecisionRate",
        descLoc: "Menu/Settings/Description/PrecisionRate")]
    [ConfigRange(0f, 5f, 500)]
    public float FlightInputPrecisionRate = 0.4f;

    [ConfigValue("Precision Decay",
        "Units per second to return accumulated precision mode pitch, yaw, and roll input toward 0.",
        nameLoc: "Menu/Settings/PrecisionDecay",
        descLoc: "Menu/Settings/Description/PrecisionDecay")]
    [ConfigRange(0f, 5f, 500)]
    public float FlightInputPrecisionGravity = 1f;

#if INTERNAL
    [ConfigSection("Thermals", loc: "Menu/Settings/Sections/Thermals")]
    [ConfigValue("Heat Shield Ablation Flux Exponent",
        "The main tuning value for how harshly heat shield ablation scales at high reentry flux.")]
    [ConfigRange(0.1, 1.0, 181, "{0:F3}")]
    public double HeatShieldAblationFluxExponent = 0.5;
#endif
}

// ReduxLib itself is a monobehaviour because it needs to be
public class ReduxLib : MonoBehaviour
{
    public const string REDUX_FOLDER = "Redux";
    private const string CONFIG_LOCATION = "./Redux/config.json";

    public static ReduxLib Instance { get; private set; } = null!;
    public static ILogProvider ReduxLogProvider;

    internal static ILogger Logger;

    /*
     * The following are going to be used for setting up the global configuration and setting up loggers
     */

    public static event Action? OnReduxLibInitialized;

    public static IConfigFile ReduxCoreConfig;

    internal static readonly ReduxLibConfig Config = new();

    public static string TimestampFormat => Config.LogTimestampFormat;
    public static bool DisablePhotosensitivityWarning => Config.DisablePhotosensitivityWarning;

    public static float FlightInputNormalSensitivity => Config.FlightInputNormalSensitivity;
    public static float FlightInputNormalGravity => Config.FlightInputNormalGravity;
    public static float FlightInputPrecisionRate => Config.FlightInputPrecisionRate;
    public static float FlightInputPrecisionGravity => Config.FlightInputPrecisionGravity;
#if INTERNAL
    public static double HeatShieldAblationFluxExponent => Config.HeatShieldAblationFluxExponent;
#else
    public static double HeatShieldAblationFluxExponent => 0.5;
#endif


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void PreInitializeReduxLib()
    {
        if (!Directory.Exists("./Redux/Config"))
        {
            Directory.CreateDirectory("./Redux/Config");
        }
        ReduxCoreConfig = new JsonConfigFile(CONFIG_LOCATION);
        ReduxCoreConfig.Bind(Config);

        ReduxLogProvider = new UnityLogProvider
        {
            CurrentFilterLevel = Config.FilterLogLevel.Value,
        };
        Physics.autoSyncTransforms = Config.UsePhysicsAutosync.Value;

        Config.FilterLogLevel.RegisterCallback((_, to) => ReduxLogProvider.CurrentFilterLevel = to);
        Config.UsePhysicsAutosync.RegisterCallback((_, to) => Physics.autoSyncTransforms = to);
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
