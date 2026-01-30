# TROUBLESHOOTING.md — Diagnóstico e Soluções

## Problema: Arquivo não encontrado ao iniciar jogo

### Sintomas
```
FileNotFoundException: Could not find file '/system/...' 
ou
Assets missing after installation
```

### Causas possíveis
1. ContentRoot não configurado corretamente
2. Arquivo não foi extraído ou foi deletado
3. Path relativo não foi resolvido

### Solução
1. Verificar LogSystem: procurar por "ContentRoot" nos logs de boot
2. Executar CheckContent novamente via UI ("Reinstalar Assets")
3. Verificar permissões da pasta:
   ```bash
   adb shell ls -la /sdcard/Android/data/Celestemeown.app/files/Celeste/
   ```

---

## Problema: XNB não carrega

### Sintomas
```
ContentLoadException: Missing asset 'Fonts/pixel_font'
ou
ExternalFileContentManager: File not found for asset 'GFX'
```

### Causas possíveis
1. ExternalFileContentManager não implementado ainda
2. Extensão .xnb não está sendo adicionada ao caminho
3. Asset não foi extraído do Content.zip

### Solução
1. Confirmar que Content.zip contém estrutura correta:
   ```bash
   unzip -l Content.zip | grep -E '\.xnb|\.fnt|\.bank'
   ```
2. Verificar logs de extração: procurar por "Extract complete" ou erros de Zip Slip
3. Se falhar: reimplantar Content.zip manualmente:
   ```bash
   adb push Content.zip /sdcard/Android/data/Celestemeown.app/files/Celeste/
   adb shell mkdir -p /sdcard/Android/data/Celestemeown.app/files/Celeste/Content
   # E executar unzip lá
   ```

---

## Problema: FMOD não inicializa (sem áudio)

### Sintomas
```
FMOD init failed: result = 1 (ERR_INVALID_PARAM)
ou
libfmod.so not found
```

### Causas possíveis
1. .so files (libfmod.so, libfmodstudio.so) não estão em jniLibs/arm64-v8a/
2. DllImport name mismatch (ex: "fmod" vs "libfmod")
3. FMOD bancos não estão na pasta correta

### Solução
1. Verificar estrutura de .so:
   ```bash
   adb shell ls -la /data/data/Celestemeown.app/lib/arm64-v8a/
   # Deve conter: libfmod.so, libfmodstudio.so
   ```
2. Se faltarem, recopiar do pacote fmodstudioapi20312android:
   - Extrair: https://github.com/portceleste8-sketch/CELESTE-GAME-ANDROID-/releases/download/V1/fmodstudioapi20312android.tar.gz
   - Copiar apenas: `fmodstudioapi20312android/build/android/arm64-v8a/*.so`
   - Para: `src/Celeste.Android/Properties/Android/jniLibs/arm64-v8a/`
3. Verificar nomes exatos nos DllImport:
   ```csharp
   [DllImport("fmod")]           // Android resolve para libfmod.so
   [DllImport("fmodstudio")]     // Android resolve para libfmodstudio.so
   ```
4. Verificar path de bancos no LogSystem
5. Se ainda falhar, aumentar verbosity:
   ```csharp
   FMOD.Debug.Initialize(
       DEBUG_CALLBACK.LOG,
       DEBUG_MODE.TTY);
   ```

---

## Problema: App trava ao iniciar

### Sintomas
```
App para de responder (ANR) após clicar "Iniciar Jogo"
ou
Exceção em CelesteGameActivity.Initialize()
```

### Causas possíveis
1. Reflexão (Assembly.GetTypes()) sem preservação de tipos (linker issue)
2. Carregamento síncrono bloqueador em Initialize
3. Exceção não tratada em Engine.Initialize

### Solução
1. Verificar LogSystem para stacktrace completo
2. Desabilitar trimming temporariamente (editar .csproj):
   ```xml
   <PropertyGroup>
       <PublishTrimmed>false</PublishTrimmed>
       <EnableLinking>false</EnableLinking>
   </PropertyGroup>
   ```
3. Se for Assembly.GetTypes(), adicionar regra de preservação:
   ```xml
   <ItemGroup>
       <RuntimeHostConfigurationOption Include="System.Runtime.TrimmerRootAssembly" Value="true" />
   </ItemGroup>
   ```
4. Mover IO pesado para thread background se necessário
5. Adicionar try/catch em cada ponto crítico com logs detalhados

---

## Problema: Controles não respondem

### Sintomas
```
Teclado físico não funciona
ou
GamePad não é detectado
```

### Causas possíveis
1. MInput.cs não está sendo chamado
2. KeyEvent não foi forwarded para GameActivity
3. GamePad requer permissão Bluetooth

### Solução
1. Testar com adb:
   ```bash
   adb shell getevent  # Plug periférico e testar
   ```
2. Verificar que Monocle/MInput.cs está sendo executado em Update()
3. Confirmar que Kotlin está forwardando events:
   ```kotlin
   override fun onKeyDown(keyCode: Int, event: KeyEvent?): Boolean {
       // Forward para MonoGame
       return super.onKeyDown(keyCode, event)
   }
   ```
4. Se for Bluetooth, adicionar permissão e pedir em runtime:
   ```xml
   <uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />
   <uses-permission android:name="android.permission.BLUETOOTH_SCAN" />
   ```

---

## Problema: Logs não aparecem/não salvam

### Sintomas
```
LogSystem não escreve arquivo
ou
Crash logs desaparecem após restart
```

### Causas possíveis
1. LogsRoot não configurado ou path inválido
2. Sem permissão de escrita
3. Exception em LogSystem.Flush()

### Solução
1. Verificar path via adb:
   ```bash
   adb shell ls -la /sdcard/Android/data/Celestemeown.app/files/Celeste/Logs/
   ```
2. Verificar permissões:
   ```bash
   adb shell stat /sdcard/Android/data/Celestemeown.app/files/Celeste/Logs/
   ```
   (Deve ter rw-r--r-- ou similar)
3. Criar pasta manualmente se faltante:
   ```bash
   adb shell mkdir -p /sdcard/Android/data/Celestemeown.app/files/Celeste/Logs/$(date +%Y-%m-%d)
   ```
4. Verificar que LogSystem é inicializado ANTES de qualquer log:
   ```csharp
   LogSystem.Initialize(logRootPath);
   LogSystem.LogInfo("Boot message");  // Deve existir
   ```
5. Se crash log desaparecer, adicionar hook:
   ```csharp
   AppDomain.CurrentDomain.UnhandledException += (s, e) =>
   {
       LogSystem.CaptureException(e.ExceptionObject as Exception);
       LogSystem.FlushLogs();  // CRÍTICO: flush síncrono
   };
   ```

---

## Problema: Download de Content.zip muito lento ou timeout

### Sintomas
```
Download interrompido ou timeout após 30s
ou
"Connection timeout" no UI
```

### Causas possíveis
1. Internet lenta (4G/3G)
2. Timeout muito curto em HttpClient
3. Servidor retornando erro 503/504

### Solução
1. Aumentar timeout no Kotlin:
   ```kotlin
   val client = OkHttpClient.Builder()
       .connectTimeout(60, TimeUnit.SECONDS)
       .readTimeout(60, TimeUnit.SECONDS)
       .writeTimeout(60, TimeUnit.SECONDS)
       .build()
   ```
2. Implementar retry automático (3x com backoff)
3. Permitir cancelamento pelo usuário
4. Mostrar progresso real (bytes/total, ETA)
5. Salvar arquivo parcial e resumir se possível

---

## Problema: Fullscreen não aplica ou volta de pause sem fullscreen

### Sintomas
```
Status bar/Navigation bar aparecem ao abrir jogo
ou
Fullscreen desaparece após pause
```

### Causas possíveis
1. onWindowFocusChanged não está sendo chamado
2. Flags SYSTEM_UI_FLAG não estão sendo reaplicadas
3. Theme em AndroidManifest.xml não está correto

### Solução
1. Confirmar AndroidManifest.xml:
   ```xml
   <activity
       android:theme="@android:style/Theme.NoTitleBar.Fullscreen"
       android:screenOrientation="landscape" />
   ```
2. Adicionar hook em onWindowFocusChanged:
   ```kotlin
   override fun onWindowFocusChanged(hasFocus: Boolean) {
       super.onWindowFocusChanged(hasFocus)
       if (hasFocus) {
           window.decorView.systemUiVisibility = 
               View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY or
               View.SYSTEM_UI_FLAG_FULLSCREEN or
               View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
       }
   }
   ```
3. Reaplicar em onResume também:
   ```kotlin
   override fun onResume() {
       super.onResume()
       // Reaplicar fullscreen
   }
   ```

---

## Problema: FPS counter lag ou não atualiza

### Sintomas
```
FPS counter congela ou mostra 0
ou
Overlay não renderiza
```

### Causas possíveis
1. Draw() não está sendo chamado
2. SpriteBatch não foi inicializado
3. Font não carregou

### Solução
1. Verificar que FpsCounterEnabled foi passado corretamente:
   ```bash
   adb logcat | grep -i "fps"
   ```
2. Confirmar que Draw() é chamado todo frame (não apenas Update)
3. Usar SpriteFont padrão ou fallback se fonte não existir
4. Adicionar instrumentation:
   ```csharp
   LogSystem.LogDebug($"FPS: {gameTime.ElapsedGameTime.TotalMilliseconds}");
   ```

---

## Problema: SAF export/import não funciona

### Sintomas
```
ACTION_CREATE_DOCUMENT não retorna resultado
ou
DocumentProvider não encontra arquivo
```

### Causas possíveis
1. requestCode não é único
2. onActivityResult não está tratando corretamente
3. File uri vs content uri mismatch

### Solução
1. Implementar ActivityResultContracts (recomendado para Android 12+):
   ```kotlin
   val createDocument = registerForActivityResult(CreateDocument("application/zip")) { uri: Uri? ->
       if (uri != null) {
           // Fazer export
       }
   }
   ```
2. Se usar onActivityResult, confirmar que requestCode é único:
   ```kotlin
   const val REQUEST_CREATE_DOCUMENT = 1001
   const val REQUEST_OPEN_DOCUMENT = 1002
   ```
3. Usar FileProvider se for interno:
   ```xml
   <provider android:name="androidx.core.content.FileProvider"
       android:authorities="Celestemeown.app.fileprovider">
       <paths>
           <files-path name="logs" path="Celeste/Logs/" />
       </paths>
   </provider>
   ```

---

## Debug rápido via ADB

### Puxar logs persistentes
```bash
adb pull /sdcard/Android/data/Celestemeown.app/files/Celeste/Logs/ ./local_logs/
```

### Ver logcat em tempo real
```bash
adb logcat | grep -i "celeste"
```

### Listar estrutura de app
```bash
adb shell ls -la /data/data/Celestemeown.app/
adb shell ls -la /sdcard/Android/data/Celestemeown.app/files/
```

### Enviar arquivo de teste
```bash
adb push Content.zip /sdcard/Download/
```

### Resetar app (limpar dados)
```bash
adb shell pm clear Celestemeown.app
```

---

## Contato e Relatório

Qualquer problema não listado acima deve ser:
1. Registrado no RELATORIO.md com detalhes exatos
2. Capturado no LogSystem (logs salvos em app-specific storage)
3. Exportado via UI (SAF export) para análise

