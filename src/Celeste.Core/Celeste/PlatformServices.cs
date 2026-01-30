using System;
using System.Collections.Generic;

namespace Celeste;

/// <summary>
/// Serviço de resolução de paths específico da plataforma.
/// Implementação separada para Desktop vs Android.
/// </summary>
public interface IPlatformPaths
{
    /// <summary>
    /// Raiz do diretório de conteúdo (assets).
    /// No Android: Context.getExternalFilesDir(null)/Celeste/Content/
    /// </summary>
    string ContentRoot { get; }

    /// <summary>
    /// Raiz do diretório de logs persistentes.
    /// No Android: Context.getExternalFilesDir(null)/Celeste/Logs/
    /// </summary>
    string LogsRoot { get; }

    /// <summary>
    /// Raiz do diretório de saves.
    /// No Android: Context.getFilesDir()/Celeste/Saves/
    /// </summary>
    string SavesRoot { get; }

    /// <summary>
    /// Raiz do diretório temporário.
    /// </summary>
    string TempRoot { get; }

    /// <summary>
    /// Resolver caminho relativo para absoluto dentro de ContentRoot.
    /// </summary>
    string ResolvePath(string relativePath);
}

/// <summary>
/// Serviço de logging completo com persistência.
/// </summary>
public interface ILogSystem
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message, Exception ex = null);
    void LogDebug(string message);

    void FlushLogs();
    void CaptureException(Exception ex);

    IReadOnlyList<string> GetSessionLogs(DateTime date);
    IReadOnlyList<string> GetCrashLogs(DateTime date);
}

/// <summary>
/// Gerenciador de conteúdo customizado para carregar XNBs do filesystem.
/// </summary>
public interface IExternalContentManager
{
    T Load<T>(string assetName) where T : class;
    void Unload();
}

/// <summary>
/// Validador de conteúdo instalado.
/// </summary>
public interface IContentValidator
{
    /// <summary>
    /// Validar presença de conteúdo crítico.
    /// </summary>
    (bool valid, List<string> missingItems) ValidateContent();

    /// <summary>
    /// Obter lista de itens críticos esperados.
    /// </summary>
    List<string> GetRequiredItems();
}
