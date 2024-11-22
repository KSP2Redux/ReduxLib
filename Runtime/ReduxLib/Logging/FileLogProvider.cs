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

    private readonly ConcurrentQueue<string> _synchronizedLogs = new();

    internal void WriteLog(string name, LogLevel level, object message)
    {
        if (level > CurrentFilterLevel) return;
        var now = DateTime.Now;
        var fullMessage = $"[{now.ToString(TimestampFormat)}] [{level.AsString()}] [{name}] {message}";
        if (MirrorToUnityLog)
        {
            Debug.unityLogger.Log(level.AsLogType(),fullMessage);
        }
        _synchronizedLogs.Enqueue(fullMessage);
    }
    
    // This should be done every frame, by some update cycle
    internal void FlushLogs()
    {
        if (_synchronizedLogs.IsEmpty) return;
        // TODO: Figure out how to constantly have a log file open?
        
        _cachedSb.Clear();
        while (_synchronizedLogs.TryDequeue(out var nextMessage))
        {
            _cachedSb.AppendLine(nextMessage);
        }
        
        File.AppendAllText(_file, _cachedSb.ToString());
    }

    ~FileLogProvider()
    {
        FlushLogs();
    }
}