using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReduxLib.Logging;

public class FileLogProvider : ILogProvider
{
    // private StreamWriter _logStream;

    // Both of these will be set in configuration, or by the person constructing the stream
    public LogLevel CurrentFilterLevel = LogLevel.Info;
    public bool MirrorToUnityLog = true;
    public string TimestampFormat = "MM/dd/yyyy HH:mm:ss";

    public event Action<LogLevel, ILogger, object>? OnLog;
    private string _file;
    private StringBuilder _cachedSb = new();
    
    public FileLogProvider(string logFile)
    {
        Debug.Log($"Creating redux log file at {logFile}");
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
        if (level > CurrentFilterLevel) return;
        _synchronizedLogs.Enqueue((level,source,message));
        OnLog?.Invoke(level, source, message);
    }
    
    // This should be done every frame, by some update cycle
    internal void FlushLogs()
    {
        if (_synchronizedLogs.IsEmpty) return;
        var now = DateTime.Now;
        // TODO: Figure out how to constantly have a log file open?
        
        _cachedSb.Clear();
        while (_synchronizedLogs.TryDequeue(out var nextMessage))
        {
            var line =
                $"[{now.ToString(TimestampFormat)}] [{nextMessage.Item1.AsString()} : {nextMessage.Item2.Name}] {nextMessage.Item3}";
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