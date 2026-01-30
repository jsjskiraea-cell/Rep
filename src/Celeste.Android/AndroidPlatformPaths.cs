using System;
using System.IO;
using Celeste;

namespace Celeste.Android
{
    // Implementação Android para IPlatformPaths (simples, usa caminhos passados pelo host)
    public class AndroidPlatformPaths : IPlatformPaths
    {
        private readonly string _contentRoot;
        private readonly string _logsRoot;
        private readonly string _savesRoot;
        private readonly string _tempRoot;

        public AndroidPlatformPaths(string contentRoot, string logsRoot, string? savesRoot = null, string? tempRoot = null)
        {
            _contentRoot = contentRoot ?? throw new ArgumentNullException(nameof(contentRoot));
            _logsRoot = logsRoot ?? Path.Combine(Path.GetDirectoryName(_contentRoot) ?? ".", "Logs");
            _savesRoot = savesRoot ?? Path.Combine(Path.GetDirectoryName(_contentRoot) ?? ".", "Saves");
            _tempRoot = tempRoot ?? Path.Combine(Path.GetTempPath(), "Celeste");
        }

        public string ContentRoot => _contentRoot;

        public string LogsRoot => _logsRoot;

        public string SavesRoot => _savesRoot;

        public string TempRoot => _tempRoot;

        public string ResolvePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return _contentRoot;
            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            return Path.Combine(_contentRoot, relativePath);
        }
    }
}
