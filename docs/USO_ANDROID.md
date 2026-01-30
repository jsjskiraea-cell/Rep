# USO_ANDROID.md — Guia de Estrutura e Fluxo Android

## Estrutura do Projeto

```
Celeste/
├── Celeste.sln                       (Solução raiz)
├── RELATORIO.md                      (Relatório único - atualizado continuamente)
├── src/
│   ├── Celeste.Core/
│   │   ├── Celeste.Core.csproj      (net8.0 - código agnóstico)
│   │   ├── Celeste/                  (~623 arquivos - lógica do jogo)
│   │   ├── Celeste.Editor/           (~88 arquivos - editor)
│   │   ├── Celeste.Pico8/            (~26 arquivos - Pico-8)
│   │   ├── FMOD/                     (bindings FMOD C#)
│   │   ├── FMOD.Studio/              (bindings FMOD Studio C#)
│   │   ├── Monocle/                  (~103 arquivos - motor)
│   │   ├── SimplexNoise/             (~4 arquivos - ruído Perlin)
│   │   └── Celeste/PlatformServices.cs  (interfaces multiplataforma)
│   │
│   └── Celeste.Android/
│       ├── Celeste.Android.csproj    (net8.0-android - app Android)
│       └── CelesteGameActivity.cs    (GameActivity MonoGame C#)
│
└── docs/
    ├── USO_ANDROID.md                (este arquivo)
    ├── LOGS.md                       (logging e diagnostics)
    └── TROUBLESHOOTING.md            (FAQ e soluções)
```

## Fluxo de Inicialização

### 1. App Launch (Kotlin/Flutter)
```
MainLauncherActivity (Kotlin)
  ↓ (hospeda)
Flutter UI (dark mode, landscape, fullscreen)
  ↓ (clica em "Iniciar Jogo")
Platform Channel → Kotlin
  ↓
GameActivity (MonoGame C# GameActivity)
  ↓
CelesteGameActivity.Initialize()
  → Recebe ContentRootPath, LogsRootPath, flags
  → Inicia Monocle/Engine
  → Inicia jogo
```

### 2. Asset Installation
```
Flutter UI → Kotlin → Download Content.zip
  ↓ (validar integridade)
  → Extração segura (Zip Slip protection)
  → Context.getExternalFilesDir(null)/Celeste/Content/
  → CheckContent (validar críticos)
  → Persistir estado em SharedPreferences
  ↓
"Assets instalados com sucesso"
```

### 3. Game Lifecycle
```
CelesteGameActivity.Initialize()
  → IPlatformPaths (injetar ContentRoot)
  ↓
Monocle/Engine.Initialize()
  → Obter ContentRoot via IPlatformPaths
  → Configurar ILogSystem
  ↓
Celeste.Game.LoadContent()
  → ExternalFileContentManager carrega XNBs do filesystem
  → Atlas/Fonts/Effects do ContentRoot
  ↓
Loop do jogo (Update/Draw)
  → FPS Counter (se habilitado)
  → LogSystem (logs de runtime)
```

## Serviços Chave

### IPlatformPaths
Abstração de caminhos específicos da plataforma.

**Implementação Android:**
```csharp
public class AndroidPlatformPaths : IPlatformPaths
{
    private readonly Context _context;
    
    public string ContentRoot =>
        Path.Combine(_context.GetExternalFilesDir(null), "Celeste", "Content");
    public string LogsRoot =>
        Path.Combine(_context.GetExternalFilesDir(null), "Celeste", "Logs");
    public string SavesRoot =>
        Path.Combine(_context.FilesDir.AbsolutePath, "Celeste", "Saves");
    public string TempRoot =>
        Path.Combine(_context.CacheDir.AbsolutePath, "Celeste", "Temp");
}
```

**Uso no Monocle/Engine:**
```csharp
public static class Engine
{
    private static IPlatformPaths _platformPaths;
    
    public static string ContentDirectory
    {
        get => Path.Combine(_platformPaths.ContentRoot, "Content");
    }
    
    public static void Initialize(IPlatformPaths platformPaths)
    {
        _platformPaths = platformPaths;
    }
}
```

### ILogSystem
Logging centralizado com persistência.

**Diretório de logs:**
- Android: `/sdcard/Android/data/Celestemeown.app/files/Celeste/Logs/YYYY-MM-DD/`
- Arquivos: `session_YYYY-MM-DD_HH-mm-ss.log`, `crash_YYYY-MM-DD_HH-mm-ss.log`

**O que logar (obrigatório):**
- Boot: device, Android version, app version, ABI
- Assets: download (progress, size), extração, validação
- Jogo: inicialização, FPS, erros, saves
- Crashes: stacktrace completo, device state, logs até o crash

### IExternalContentManager
ContentManager customizado para carregar XNBs do filesystem.

```csharp
public class ExternalFileContentManager : IExternalContentManager
{
    private readonly string _contentRoot;
    private readonly Dictionary<string, object> _cache = new();
    
    public T Load<T>(string assetName) where T : class
    {
        // 1. Validar assetName (sem ../ ou paths absolutos)
        // 2. Procurar em _contentRoot
        // 3. Abrir arquivo via FileStream
        // 4. Desserializar XNB para tipo T
        // 5. Cacher e retornar
    }
}
```

### IContentValidator
Validação de assets após instalação/extração.

**Itens críticos esperados:**
- Diretórios: Dialog/, Fonts/, Effects/, Atlases/, Audio/Banks/
- Arquivos específicos: GFX.xnb (Effect), pixel_font.fnt, dialogue_pt.txt
- Padrão de bins/data: *.bin/*.data (Atlas data)
- Bancos FMOD: *.bank (se houver em FMOD/Android/)

## Permissões Android (Mínimo)

```xml
<manifest>
  <!-- Necessárias -->
  <uses-permission android:name="android.permission.INTERNET" />
  <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
  
  <!-- Opcionais (apenas se implementados) -->
  <uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
  <!-- Bluetooth: somente se necessário, pedir em runtime -->
</manifest>
```

## Armazenamento (Sem MANAGE_EXTERNAL_STORAGE)

**App-specific external storage:**
```
Context.getExternalFilesDir(null)
  → /sdcard/Android/data/Celestemeown.app/files/
  → Autorremovido ao desinstalar o app
  → Não requer MANAGE_EXTERNAL_STORAGE
```

**App-specific internal storage:**
```
Context.getFilesDir()
  → /data/data/Celestemeown.app/files/
  → Private, apenas para este app
  → Salva (Saves, configuration)
```

**Export/Import (sem READ/WRITE_EXTERNAL_STORAGE):**
- Usar SAF (Storage Access Framework)
- ACTION_CREATE_DOCUMENT para salvar
- ACTION_OPEN_DOCUMENT para carregar
- User choose destino/origem

## Configuração Kotlin no Android (AndroidManifest.xml)

```xml
<activity android:name=".MainLauncherActivity"
    android:exported="true"
    android:screenOrientation="landscape"
    android:theme="@android:style/Theme.NoTitleBar.Fullscreen">
    <intent-filter>
        <action android:name="android.intent.action.MAIN" />
        <category android:name="android.intent.category.LAUNCHER" />
    </intent-filter>
</activity>

<activity android:name=".GameActivity"
    android:screenOrientation="landscape"
    android:theme="@android:style/Theme.NoTitleBar.Fullscreen" />
```

## Fullscreen e Landscape (Obrigatório)

```kotlin
class GameActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        
        // Landscape
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE
        
        // Fullscreen imersivo
        window.decorView.systemUiVisibility = 
            View.SYSTEM_UI_FLAG_LAYOUT_STABLE or
            View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION or
            View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN or
            View.SYSTEM_UI_FLAG_HIDE_NAVIGATION or
            View.SYSTEM_UI_FLAG_FULLSCREEN or
            View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
    }
    
    override fun onWindowFocusChanged(hasFocus: Boolean) {
        super.onWindowFocusChanged(hasFocus)
        if (hasFocus) {
            // Reaplica fullscreen ao retornar de pause
            window.decorView.systemUiVisibility = 
                View.SYSTEM_UI_FLAG_LAYOUT_STABLE or
                View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION or
                View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN or
                View.SYSTEM_UI_FLAG_HIDE_NAVIGATION or
                View.SYSTEM_UI_FLAG_FULLSCREEN or
                View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
        }
    }
}
```

## Próximas Etapas

1. ETAPA 2: Adaptar Monocle/Engine.cs para usar IPlatformPaths
2. ETAPA 3: Implementar ExternalFileContentManager
3. ETAPA 4: Implementar LogSystem completo
4. ETAPA 5: Integrar download/instalação de assets (Kotlin)
5. ETAPA 6: Integração Flutter UI
6. ETAPA 7: FMOD Android (arm64-v8a)
7. ETAPA 8: FPS Counter e controles na tela
8. ETAPA 9: SAF export/import
9. ETAPA 10: Testes e ajustes finais

