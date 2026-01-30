using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Celeste;

public class FileLogSystem : ILogSystem
{
    private readonly string _logsRoot;
    private readonly object _lock = new object();
    private readonly List<string> _buffer = new List<string>();
    private string _currentSessionFile;

    public FileLogSystem(string logsRoot)
    {
        _logsRoot = logsRoot ?? throw new ArgumentNullException(nameof(logsRoot));
        Directory.CreateDirectory(_logsRoot);
        StartSession();
    }

    private void StartSession()
    {
        var date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var dir = Path.Combine(_logsRoot, date);
        Directory.CreateDirectory(dir);
        _currentSessionFile = Path.Combine(dir, $"session_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.log");
        AppendLine($"[ {DateTime.UtcNow:O} ] ======== SESSION START ========");
        FlushLogs();
    }

    private void AppendLine(string line)
    {
        lock (_lock)
        {
            _buffer.Add(line);
        }
    }

    public void LogInfo(string message)
    {
        AppendLine($"[ {DateTime.UtcNow:O} ] INFO: {message}");
    }

    public void LogWarning(string message)
    {
        AppendLine($"[ {DateTime.UtcNow:O} ] WARN: {message}");
    }

    public void LogError(string message, Exception ex = null)
    {
        AppendLine($"[ {DateTime.UtcNow:O} ] ERROR: {message} {ex?.ToString()}");
    }

    public void LogDebug(string message)
    {
        AppendLine($"[ {DateTime.UtcNow:O} ] DEBUG: {message}");
    }

    public void FlushLogs()
    {
        List<string> items;
        lock (_lock)
        {
            if (_buffer.Count == 0) return;
            items = new List<string>(_buffer);
            _buffer.Clear();
        }
        try
        {
            File.AppendAllLines(_currentSessionFile, items, Encoding.UTF8);
        }
        catch
        {
            // swallow to avoid crashes in logging
        }
    }

    public void CaptureException(Exception ex)
    {
        try
        {
            var date = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var dir = Path.Combine(_logsRoot, date);
            Directory.CreateDirectory(dir);
            var crashFile = Path.Combine(dir, $"crash_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.log");
            var sb = new StringBuilder();
            sb.AppendLine($"[ {DateTime.UtcNow:O} ] CRASH");
            sb.AppendLine(ex.ToString());
            File.WriteAllText(crashFile, sb.ToString(), Encoding.UTF8);
        }
        catch
        {
            // ignore
        }
        finally
        {
            FlushLogs();
        }
    }

    public IReadOnlyList<string> GetSessionLogs(DateTime date)
    {
        var dir = Path.Combine(_logsRoot, date.ToString("yyyy-MM-dd"));
        if (!Directory.Exists(dir)) return Array.Empty<string>();
        return Directory.GetFiles(dir, "session_*.log").OrderBy(x => x).ToList();
    }

    public IReadOnlyList<string> GetCrashLogs(DateTime date)
    {
        var dir = Path.Combine(_logsRoot, date.ToString("yyyy-MM-dd"));
        if (!Directory.Exists(dir)) return Array.Empty<string>();
        return Directory.GetFiles(dir, "crash_*.log").OrderBy(x => x).ToList();
    }
}
