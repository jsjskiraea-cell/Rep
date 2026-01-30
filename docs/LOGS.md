# LOGS.md — Sistema de Logs e Diagnóstico

## Estrutura de Logs

### Diretório base
```
Android: /sdcard/Android/data/Celestemeown.app/files/Celeste/Logs/
Local: Context.getExternalFilesDir(null) + "/Celeste/Logs/"
```

### Organização por data
```
Celeste/Logs/
├── 2026-01-30/
│   ├── session_2026-01-30_09-15-23.log
│   ├── session_2026-01-30_14-30-45.log
│   └── crash_2026-01-30_16-22-11.log
├── 2026-01-31/
│   ├── session_2026-01-31_07-45-12.log
│   └── ...
```

## Tipos de Log

### Session Log (session_YYYY-MM-DD_HH-mm-ss.log)
Arquivo de log de uma sessão normal da aplicação.

**Conteúdo:**
```
[2026-01-30 09:15:23] ========== BOOT LOG ==========
[2026-01-30 09:15:23] Device: samsung SM-G9810 (arm64-v8a)
[2026-01-30 09:15:23] Android Version: 13 (SDK 33)
[2026-01-30 09:15:23] App Version: 1.0 Build: 1
[2026-01-30 09:15:23] Content Root: /sdcard/Android/data/Celestemeown.app/files/Celeste/Content
[2026-01-30 09:15:23] Logs Root: /sdcard/Android/data/Celestemeown.app/files/Celeste/Logs
[2026-01-30 09:15:23]
[2026-01-30 09:15:24] ========== ASSET VALIDATION ==========
[2026-01-30 09:15:24] CheckContent: Starting validation
[2026-01-30 09:15:24] ✓ Dialog/ (234 files)
[2026-01-30 09:15:24] ✓ Fonts/ (3 files)
[2026-01-30 09:15:24] ✓ Effects/ (1 file: GFX.xnb)
[2026-01-30 09:15:24] ✓ Atlases/ (156 entries, .bin/.data/.meta/.png)
[2026-01-30 09:15:24] ✓ Audio/Banks/ (8 files: *.bank)
[2026-01-30 09:15:24] CheckContent: PASSED (assets valid)
[2026-01-30 09:15:24] Persisting validation state: assets_instalados=true, validation_ok=true
[2026-01-30 09:15:24]
[2026-01-30 09:15:25] ========== FMOD INITIALIZATION ==========
[2026-01-30 09:15:25] FMOD.Studio.System.Initialize() starting
[2026-01-30 09:15:25] FMOD Studio System initialized (maxchannels=32)
[2026-01-30 09:15:25] Loading bank: Celeste/Logs/../../Content/Audio/Banks/Master.bank
[2026-01-30 09:15:25] ✓ Bank loaded: Master.bank (1.2 MB)
[2026-01-30 09:15:25] Loading bank: Celeste/Logs/../../Content/Audio/Banks/Master.strings.bank
[2026-01-30 09:15:25] ✓ Bank loaded: Master.strings.bank (45 KB)
[2026-01-30 09:15:25] FMOD initialization COMPLETE
[2026-01-30 09:15:25]
[2026-01-30 09:15:26] ========== MONOGAME INITIALIZATION ==========
[2026-01-30 09:15:26] MonoGame version: 3.8.1.1379
[2026-01-30 09:15:26] GraphicsAdapter: Mali-G78 MP20
[2026-01-30 09:15:26] BackBuffer: 2560x1440 (landscape)
[2026-01-30 09:15:26] Fullscreen: true (immersive sticky)
[2026-01-30 09:15:26]
[2026-01-30 09:15:27] ========== CELESTE GAME INITIALIZATION ==========
[2026-01-30 09:15:27] Monocle Engine starting
[2026-01-30 09:15:27] Loading Celeste.Game...
[2026-01-30 09:15:28] Content load: Dialog/english.txt (45 KB)
[2026-01-30 09:15:28] Content load: Fonts/pixel_font.fnt (78 KB)
[2026-01-30 09:15:28] Content load: Fonts/pixel_font.png (1.2 MB)
[2026-01-30 09:15:28] XNB load: Effects/GFX.xnb (deserialized Effect)
[2026-01-30 09:15:28] Atlas load: Celeste/player (2048x2048 atlas, 156 sprites)
[2026-01-30 09:15:29] ✓ Game loaded (total time: 3.2s)
[2026-01-30 09:15:29] FPS Counter: ENABLED
[2026-01-30 09:15:29] Verbose Logs: ENABLED
[2026-01-30 09:15:29]
[2026-01-30 09:15:29] ========== RUNTIME LOG ==========
[2026-01-30 09:15:30] Game started, entering loop
[2026-01-30 09:15:31] [FPS] avg=60.0 min=58 max=62 (1s)
[2026-01-30 09:15:32] [FPS] avg=59.8 min=58 max=61 (1s)
[2026-01-30 09:15:33] [FPS] avg=60.1 min=59 max=61 (1s)
[2026-01-30 09:15:34] Save file written: Saves/backup_slot_0.sav (234 KB)
[2026-01-30 09:15:35] [FPS] avg=59.9 min=59 max=61 (1s)
[2026-01-30 09:15:40] User exited game (back button pressed)
[2026-01-30 09:15:40] OnExiting: flushing saves and logs...
[2026-01-30 09:15:40] ========== SESSION END ==========
[2026-01-30 09:15:40] Total runtime: 25.3s
```

### Crash Log (crash_YYYY-MM-DD_HH-mm-ss.log)
Arquivo gerado automaticamente quando ocorre exceção não tratada.

**Conteúdo:**
```
[2026-01-30 16:22:11] ========== CRASH LOG ==========
[2026-01-30 16:22:11] Timestamp: 2026-01-30 16:22:11 UTC
[2026-01-30 16:22:11] Device: samsung SM-G9810 (arm64-v8a)
[2026-01-30 16:22:11] Android Version: 13 (SDK 33)
[2026-01-30 16:22:11]
[2026-01-30 16:22:11] ========== EXCEPTION ==========
[2026-01-30 16:22:11] Exception Type: NullReferenceException
[2026-01-30 16:22:11] Exception Message: Object reference not set to an instance of an object.
[2026-01-30 16:22:11] Exception StackTrace:
  at Celeste.Player.Update() in Celeste/Player.cs:line 456
  at Monocle.Engine.Update(GameTime gameTime) in Monocle/Engine.cs:line 289
  at CelesteGameActivity.Update(GameTime gameTime) in CelesteGameActivity.cs:line 45
[2026-01-30 16:22:11]
[2026-01-30 16:22:11] ========== INNER EXCEPTION ==========
[2026-01-30 16:22:11] (none)
[2026-01-30 16:22:11]
[2026-01-30 16:22:11] ========== STATE AT CRASH ==========
[2026-01-30 16:22:11] Last game action: Player.Jump()
[2026-01-30 16:22:11] Current level: Prologue-A (checkpoint: 1)
[2026-01-30 16:22:11] Player position: (234.5, 123.8)
[2026-01-30 16:22:11] Uptime: 12.4s
[2026-01-30 16:22:11] FPS enabled: true
[2026-01-30 16:22:11] Assets validated: true
[2026-01-30 16:22:11]
[2026-01-30 16:22:11] ========== SYSTEM STATE ==========
[2026-01-30 16:22:11] Memory: 1.2 GB / 8 GB used
[2026-01-30 16:22:11] Battery: 75% (discharging)
[2026-01-30 16:22:11] CPU: 18% usage
[2026-01-30 16:22:11]
[2026-01-30 16:22:11] ========== RECENT LOGS (last 50 lines) ==========
[2026-01-30 16:22:06] [FPS] avg=60.1 min=59 max=62 (1s)
[2026-01-30 16:22:07] [FPS] avg=59.9 min=59 max=61 (1s)
[2026-01-30 16:22:08] [FPS] avg=60.2 min=60 max=61 (1s)
[2026-01-30 16:22:09] Input: KeyCode.Space pressed (jump)
[2026-01-30 16:22:10] [FPS] avg=60.0 min=59 max=61 (1s)
... (mais linhas do session log)
[2026-01-30 16:22:11] ========== CRASH END ==========
```

## Logging no Código (ILogSystem)

### Inicialização
```csharp
// Em CelesteGameActivity.Initialize()
var platformPaths = new AndroidPlatformPaths(context);
var logSystem = new FileLogSystem(platformPaths.LogsRoot);
logSystem.Initialize();

// Registrar handlers de exceção
AppDomain.CurrentDomain.UnhandledException += (s, e) =>
{
    logSystem.CaptureException(e.ExceptionObject as Exception);
    logSystem.FlushLogs();  // CRÍTICO: flush síncrono em crash
};
```

### Uso durante runtime
```csharp
// Info
LogSystem.LogInfo("Game initialized successfully");

// Debug (só se verbose logs habilitado)
LogSystem.LogDebug($"FPS: {gameTime.ElapsedGameTime}");

// Warning
LogSystem.LogWarning("Asset not found: Fonts/pixel_font.fnt (fallback used)");

// Error
LogSystem.LogError("Failed to load save file", exception);

// Flush controlado
LogSystem.FlushLogs();  // Ao sair do game
```

## Coleta via ADB

### Puxar todos os logs de hoje
```bash
adb pull /sdcard/Android/data/Celestemeown.app/files/Celeste/Logs/$(date +%Y-%m-%d) ./logs_today/
```

### Puxar logs de data específica
```bash
adb pull /sdcard/Android/data/Celestemeown.app/files/Celeste/Logs/2026-01-30 ./logs_jan30/
```

### Ver último crash log em tempo real
```bash
adb shell ls -t /sdcard/Android/data/Celestemeown.app/files/Celeste/Logs/*/ | grep crash | head -1 | xargs adb pull
```

### Monitorar logcat ao vivo
```bash
adb logcat | grep -E "Celeste|FMOD|MonoGame|GameActivity"
```

### Exportar logcat para arquivo
```bash
adb logcat > logcat_dump.txt 2>&1
# Deixar rodando enquanto repete o bug
Ctrl+C para parar
```

## Análise de Logs

### Buscar erros específicos
```bash
# Procurar por exceções
grep -i "exception\|error\|failed" Logs/*/*.log

# Procurar por problemas de FMOD
grep -i "fmod" Logs/*/*.log

# Procurar por problemas de asset
grep -i "asset\|not found\|missing" Logs/*/*.log
```

### Extrair seção específica
```bash
# Boot log (primeiros 50 linhas de um session log)
head -50 Logs/2026-01-30/session_2026-01-30_09-15-23.log

# Runtime (ignorar boot, pegar FPS e eventos)
grep -E "^\[.*\] (\[FPS\]|Save|Input|Error)" Logs/2026-01-30/session_2026-01-30_09-15-23.log

# Crash completo
cat Logs/2026-01-30/crash_2026-01-30_16-22-11.log
```

## Performance e Armazenamento

### Políticas de retenção (recomendado)
- Manter logs de últimas 30 dias
- Crash logs indefinidamente (ou últimos 90 dias)
- Rotar automaticamente (sharding por data)

### Tamanho esperado
- Session log (1 hora): ~5-15 MB (com verbose logs)
- Session log (1 hora): ~1-2 MB (sem verbose logs)
- Crash log: ~200-500 KB (com stacktrace completo)
- Total recomendado: ~100-200 MB para 30 dias

### Compressão (para export)
```kotlin
// Compactar logs antes de enviar via SAF
val zipFile = File(cacheDir, "celeste_logs.zip")
ZipOutputStream(FileOutputStream(zipFile)).use { zos ->
    File(getExternalFilesDir(null), "Celeste/Logs").walk().forEach { file ->
        if (file.isFile) {
            val entry = ZipEntry(file.relativeTo(logsDir).path)
            zos.putNextEntry(entry)
            file.inputStream().copyTo(zos)
            zos.closeEntry()
        }
    }
}
```

## Export via UI (SAF)

### Fluxo de exportação
1. Usuário clica "Exportar Logs" na UI Flutter
2. Flutter chama Kotlin via MethodChannel: `exportLogs()`
3. Kotlin abre ACTION_CREATE_DOCUMENT (SAF):
   - Tipo: application/zip
   - Nome padrão: `celeste_logs_2026-01-30.zip`
   - Usuário escolhe pasta
4. Kotlin cria zip dos logs de hoje (ou período selecionado)
5. Grava em Uri retornado por SAF
6. Confirma sucesso na UI
7. LogSystem registra: "Logs exportados via SAF para: <uri>"

### Exemplo Kotlin
```kotlin
private fun exportLogs() {
    val intent = Intent(Intent.ACTION_CREATE_DOCUMENT).apply {
        addCategory(Intent.CATEGORY_OPENABLE)
        type = "application/zip"
        putExtra(Intent.EXTRA_TITLE, "celeste_logs_${Date()}.zip")
    }
    startActivityForResult(intent, REQUEST_CREATE_DOCUMENT)
}

override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
    super.onActivityResult(requestCode, resultCode, data)
    if (requestCode == REQUEST_CREATE_DOCUMENT && resultCode == RESULT_OK) {
        data?.data?.let { uri ->
            // Criar zip e gravar em uri
            LogSystem.LogInfo("Logs exported to SAF URI: $uri")
        }
    }
}
```

## Import de Logs Anteriores

Não é necessário (logs são persistentes), mas suportado:
- Usuário pode enviar arquivo .zip antigo via SAF
- App extrai em pasta temporária
- Usuário visualiza/analisa no app

## Troubleshooting de Logs

Ver [TROUBLESHOOTING.md](TROUBLESHOOTING.md) seção "Problema: Logs não aparecem/não salvam"

