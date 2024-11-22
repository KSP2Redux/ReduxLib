using System;
using JetBrains.Annotations;
using UnityEngine;

namespace ReduxLib.Logging;

/// <summary>
/// The log level.
/// </summary>
[PublicAPI]
public enum LogLevel
{
    /// <summary>
    /// No logging.
    /// </summary>
    None = 0,
    /// <summary>
    /// Fatal errors.
    /// </summary>
    Fatal = 1,
    /// <summary>
    /// Errors.
    /// </summary>
    Error = 2,
    /// <summary>
    /// Warnings.
    /// </summary>
    Warning = 4,
    /// <summary>
    /// Messages.
    /// </summary>
    Message = 8,
    /// <summary>
    /// Information.
    /// </summary>
    Info = 16,
    /// <summary>
    /// Debug information.
    /// </summary>
    Debug = 32,
    /// <summary>
    /// All logging.
    /// </summary>
    All = Debug | Info | Message | Warning | Error | Fatal
}

public static class LogLevelExtensions
{
    public static string AsString(this LogLevel level)
    {
        return level switch
        {
            >= LogLevel.Debug => "DBG",
            >= LogLevel.Info => "INF",
            >= LogLevel.Message => "MSG",
            >= LogLevel.Warning => "WRN",
            >= LogLevel.Error => "ERR",
            >= LogLevel.Fatal => "FAT",
            _ => "???"
        };
    }

    public static LogType AsLogType(this LogLevel level)
    {
        
        return level switch
        {
            >= LogLevel.Debug => LogType.Log,
            >= LogLevel.Info => LogType.Log,
            >= LogLevel.Message => LogType.Log,
            >= LogLevel.Warning => LogType.Warning,
            >= LogLevel.Error => LogType.Error,
            >= LogLevel.Fatal => LogType.Error,
            _ => LogType.Log
        };
    }
}