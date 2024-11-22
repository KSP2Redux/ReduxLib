using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace ReduxLib.Logging;

public class FileLogProvider : IDisposable, IAsyncDisposable, ILogProvider
{
    private StreamWriter _logStream;

    // Both of these will be set in configuration, or by the person constructing the stream
    public LogLevel CurrentFilterLevel = LogLevel.Info;
    public bool MirrorToUnityLog = true;
    public string TimestampFormat = "MM/dd/yyyy HH:mm:ss";
    
    public FileLogProvider(string logFile)
    {
        _logStream = new StreamWriter(File.OpenWrite(logFile));
    }
    
    public void Dispose()
    {
        FlushLogs();
        _logStream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        FlushLogs();
        await _logStream.DisposeAsync();
    }

    public ILogger GetLogger(string name) => new FileLogger(name, this);

    private readonly ConcurrentQueue<string> _synchronizedLogs = new();

    internal void WriteLog(string name, LogLevel level, object message)
    {
        if (level > CurrentFilterLevel) return;
        var now = DateTime.Now;
        var fullMessage = $"[{now.ToString(TimestampFormat)}] [{name}] [{level.AsString()}] {message}";
        if (MirrorToUnityLog)
        {
            Debug.unityLogger.Log(level.AsLogType(),fullMessage);
        }
        _synchronizedLogs.Enqueue(fullMessage);
    }
    
    // This should be done every frame, by some update cycle
    internal void FlushLogs()
    {
        while (_synchronizedLogs.TryDequeue(out var nextMessage))
        {
            _logStream.WriteLine(nextMessage);
        }
    }
}