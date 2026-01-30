namespace Celeste;

/// <summary>
/// Valida que todos os assets críticos estão presentes após instalação.
/// Implementa IContentValidator.
/// </summary>
public class ContentValidator : IContentValidator
{
    private readonly string _contentRoot;
    private List<string> _missingItems = new();

    public ContentValidator(string contentRoot)
    {
        _contentRoot = contentRoot ?? throw new ArgumentNullException(nameof(contentRoot));
    }

    /// <summary>
    /// Valida todos os assets críticos.
    /// Retorna true se todos presentes, false se algum falta.
    /// </summary>
    public bool ValidateContent()
    {
        _missingItems.Clear();

        // 1. Validar diretórios
        ValidateDirectory("Dialog");
        ValidateDirectory("Fonts");
        ValidateDirectory("Effects");
        ValidateDirectory("Atlases");
        ValidateDirectory("Audio/Banks");

        // 2. Validar arquivos específicos
        ValidateFile("Fonts/pixel_font.fnt");
        ValidateFile("Fonts/pixel_font.png");
        ValidateFile("Effects/GFX.xnb");
        ValidateFile("Dialog/english.txt");

        // 3. Validar atlas entries (pelo menos um)
        if (!ValidateAtlasEntries())
        {
            _missingItems.Add("Atlases: No atlas entries found");
        }

        // 4. Validar FMOD banks (se presentes)
        ValidateFmodBanks();

        return _missingItems.Count == 0;
    }

    /// <summary>
    /// Retorna lista de items faltantes.
    /// </summary>
    public List<string> GetRequiredItems()
    {
        return new List<string>
        {
            "Dialog/",
            "Fonts/pixel_font.fnt",
            "Fonts/pixel_font.png",
            "Effects/GFX.xnb",
            "Atlases/ (at least 1 .bin/.data/.meta/.png set)",
            "Audio/Banks/ (Master.bank, Master.strings.bank)",
            "Dialog/english.txt"
        };
    }

    /// <summary>
    /// Validar que diretório existe.
    /// </summary>
    private void ValidateDirectory(string relativePath)
    {
        var dirPath = Path.Combine(_contentRoot, relativePath);
        if (!Directory.Exists(dirPath))
        {
            _missingItems.Add($"{relativePath}/ (directory not found)");
        }
    }

    /// <summary>
    /// Validar que arquivo existe.
    /// </summary>
    private void ValidateFile(string relativePath)
    {
        var filePath = Path.Combine(_contentRoot, relativePath);
        if (!File.Exists(filePath))
        {
            _missingItems.Add($"{relativePath} (file not found)");
        }
    }

    /// <summary>
    /// Validar que existe pelo menos um atlas completo (bin/data/meta/png).
    /// </summary>
    private bool ValidateAtlasEntries()
    {
        var atlasDir = Path.Combine(_contentRoot, "Atlases");
        if (!Directory.Exists(atlasDir))
            return false;

        // Procurar por padrão: *.bin, *.data, *.meta, *.png
        var binFiles = Directory.GetFiles(atlasDir, "*.bin", SearchOption.TopDirectoryOnly);
        var dataFiles = Directory.GetFiles(atlasDir, "*.data", SearchOption.TopDirectoryOnly);
        var pngFiles = Directory.GetFiles(atlasDir, "*.png", SearchOption.TopDirectoryOnly);

        // Pelo menos um set completo
        return binFiles.Length > 0 && dataFiles.Length > 0 && pngFiles.Length > 0;
    }

    /// <summary>
    /// Validar FMOD banks se pasta Audio/Banks existe.
    /// </summary>
    private void ValidateFmodBanks()
    {
        var banksDir = Path.Combine(_contentRoot, "Audio", "Banks");
        if (!Directory.Exists(banksDir))
        {
            _missingItems.Add("Audio/Banks/ (directory not found)");
            return;
        }

        var bankFiles = Directory.GetFiles(banksDir, "*.bank", SearchOption.TopDirectoryOnly);
        if (bankFiles.Length == 0)
        {
            _missingItems.Add("Audio/Banks/: No .bank files found");
        }
    }

    /// <summary>
    /// Debug: imprimir status detalhado.
    /// </summary>
    public void PrintValidationStatus()
    {
        Console.WriteLine("[ContentValidator] === VALIDATION STATUS ===");
        Console.WriteLine($"[ContentValidator] Content Root: {_contentRoot}");

        if (_missingItems.Count == 0)
        {
            Console.WriteLine("[ContentValidator] ✓ All critical assets present");
        }
        else
        {
            Console.WriteLine($"[ContentValidator] ✗ Missing {_missingItems.Count} items:");
            foreach (var item in _missingItems)
            {
                Console.WriteLine($"[ContentValidator]   - {item}");
            }
        }
    }
}
