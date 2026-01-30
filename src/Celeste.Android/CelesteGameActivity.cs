using System;
using Celeste;

// Note: This class runs in the Android host and wires platform services into the Core.

namespace Celeste.Android;

/// <summary>
/// GameActivity principal do Celeste para Android (stub para ETAPA 5).
/// Será integrado com MonoGame quando workload Android estiver disponível.
/// Inicia com base nos paths fornecidos pelo host Kotlin.
/// </summary>
public class CelesteGameActivity
{
    // Configurações passadas pela Intent/Kotlin
    public string ContentRootPath { get; set; } = string.Empty;
    public bool FpsCounterEnabled { get; set; }
    public bool VerboseLogsEnabled { get; set; }
    public bool TouchOverlayEnabled { get; set; }
    public string LogsRootPath { get; set; } = string.Empty;

    public CelesteGameActivity()
    {
        // TODO: Integrar MonoGame GameActivity (net8.0-android workload necessário)
    }

    private ExternalFileContentManager? _externalContent;

    public void Initialize()
    {
        // Garantir caminhos
        if (string.IsNullOrEmpty(ContentRootPath))
        {
            ContentRootPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                "Celeste", "Content");
        }
        if (string.IsNullOrEmpty(LogsRootPath))
        {
            LogsRootPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(ContentRootPath) ?? ".", "Logs");
        }

        // Criar e registrar PlatformPaths
        var platformPaths = new AndroidPlatformPaths(ContentRootPath, LogsRootPath);
        Monocle.Engine.SetPlatformPaths(platformPaths);

        // Inicializar ExternalContentManager (será usado pelo Engine para resolver assets)
        _externalContent = new ExternalFileContentManager(platformPaths.ContentRoot);

        Console.WriteLine($"[GameActivity] Initialized with ContentRoot: {platformPaths.ContentRoot}");
        Console.WriteLine($"[GameActivity] Logs will be saved to: {platformPaths.LogsRoot}");

        // TODO: FileLogSystem integration (ETAPA 4)
        // TODO: Integração completa com o ContentManager do MonoGame (workload Android necessário)
    }

    public void LoadContent()
    {
        // TODO: Carregar conteúdo de forma segura
        // Usar _externalContent para carregar XNBs do filesystem
    }

    public void Update(double deltaTime)
    {
        // TODO: Lógica principal do Celeste
    }

    public void Draw(double deltaTime)
    {
        // TODO: Desenhar o jogo
        if (FpsCounterEnabled)
        {
            DrawFpsCounter(deltaTime);
        }
    }

    private void DrawFpsCounter(double deltaTime)
    {
        // TODO: Implementar renderização de FPS
        // Usar fonte do jogo se disponível
    }

    public void OnExiting()
    {
        // TODO: Flush de logs
        // TODO: Salvar estado
    }
}
