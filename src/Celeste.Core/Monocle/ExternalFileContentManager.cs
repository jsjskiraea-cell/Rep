using System;
using System.Collections.Generic;
using System.IO;

namespace Celeste
{
    // Implementação mínima de IExternalContentManager que resolve assets no filesystem.
    // Nota: O carregamento real de XNBs/Texture2D/Effect precisará de desserializadores específicos do MonoGame.
    public class ExternalFileContentManager : IExternalContentManager
    {
        private readonly string _contentRoot;
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public ExternalFileContentManager(string contentRoot)
        {
            _contentRoot = contentRoot ?? throw new ArgumentNullException(nameof(contentRoot));
        }

        public T Load<T>(string assetName) where T : class
        {
            if (string.IsNullOrEmpty(assetName))
                return default;

            // Normaliza assetName e previne path traversal
            string safeName = assetName.Replace('\\', '/').TrimStart('/');
            if (safeName.Contains(".."))
                throw new InvalidOperationException("Invalid asset name");

            // Primeiro, cache
            if (_cache.TryGetValue(safeName, out object cached) && cached is T t)
                return t;

            string[] possible = new string[] { 
                Path.Combine(_contentRoot, safeName),
                Path.Combine(_contentRoot, safeName + ".xnb"),
                Path.Combine(_contentRoot, safeName + ".png"),
                Path.Combine(_contentRoot, safeName + ".fnt")
            };

            foreach (var p in possible)
            {
                if (File.Exists(p))
                {
                    // Para o momento, não desserializamos XNBs; apenas retornamos null ou a stream se T==Stream
                    if (typeof(T) == typeof(Stream))
                    {
                        var fs = File.OpenRead(p);
                        _cache[safeName] = fs;
                        return fs as T;
                    }

                    // Se for string (texto), carregar conteúdo de texto
                    if (typeof(T) == typeof(string))
                    {
                        var text = File.ReadAllText(p);
                        _cache[safeName] = text;
                        return text as T;
                    }

                    // Fallback: não suportado ainda
                    return default;
                }
            }

            return default;
        }

        public void Unload()
        {
            foreach (var v in _cache.Values)
            {
                if (v is Stream s)
                {
                    try { s.Dispose(); } catch { }
                }
            }
            _cache.Clear();
        }
    }
}
