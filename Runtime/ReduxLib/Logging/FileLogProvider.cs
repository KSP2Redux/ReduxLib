using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReduxLib.Logging;

public class FileLogProvider : ILogProvider, IUpdatableLogProvider
{
    // Both of these will be set in configuration, or by the person constructing the stream
    public LogLevel CurrentFilterLevel { get; set; } = LogLevel.Info;
    public bool MirrorToUnityLog = true;
    public event Action<LogLevel, ILogger, object>? OnLog;

    private readonly string _file;
    private readonly string _timestampFormat;
    private readonly StringBuilder _cachedSb = new();

    public FileLogProvider(string logFile, string? timestampFormat = null)
    {
        Debug.Log($"Creating log file at {logFile}");
        if (File.Exists(logFile))
        {
            if (File.Exists($"{logFile}.old"))
            {
                File.Delete($"{logFile}.old");
            }

            File.Copy(logFile, $"{logFile}.old");
            File.Delete(logFile);
        }

        _file = logFile;
        _timestampFormat = timestampFormat ?? ReduxLib.TimestampFormat;
    }

    public void Dispose()
    {
        FlushLogs();
    }

    public async ValueTask DisposeAsync()
    {
        FlushLogs();
    }

    public ILogger GetLogger(string name) => new FileLogger(name, this);

    private readonly ConcurrentQueue<(LogLevel, ILogger, object)> _synchronizedLogs = new();

    internal void WriteLog(ILogger source, LogLevel level, object message)
    {
        if (level > CurrentFilterLevel)
            return;
        _synchronizedLogs.Enqueue((level, source, message));
        OnLog?.Invoke(level, source, message);
    }

    public void Update() => FlushLogs();

    private void FlushLogs()
    {
        if (_synchronizedLogs.IsEmpty)
            return;

        DateTime now = DateTime.Now;

        // TODO: Figure out how to constantly have a log file open?

        _cachedSb.Clear();
        while (_synchronizedLogs.TryDequeue(out (LogLevel, ILogger, object) nextMessage))
        {
            string line = $"[{now.ToString(_timestampFormat)}] [{nextMessage.Item1.AsString()} : {nextMessage.Item2.Name}] {nextMessage.Item3}";
            _cachedSb.AppendLine(line);

            if (MirrorToUnityLog)
            {
                Debug.unityLogger.Log(nextMessage.Item1.AsLogType(), line);
            }
        }

        File.AppendAllText(_file, _cachedSb.ToString());
    }

    ~FileLogProvider()
    {
        FlushLogs();
    }
}