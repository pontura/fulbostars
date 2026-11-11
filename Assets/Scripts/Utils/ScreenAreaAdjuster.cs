// ScreenAreaAdjuster.cs
// Achica la imagen FINAL del juego (cámaras + post-proceso + toda la UI, incluidos Canvas Overlay)
// en horizontal y vertical por separado, y rellena el resto con negro.
// Se crea solo al arrancar y persiste entre escenas: basta con poner este archivo en Assets/.
//
// Atajos (en runtime):
//   Ctrl + Izq/Der      -> ancho  (-/+)
//   Ctrl + Abajo/Arriba -> alto   (-/+)
//   Shift + flechas     -> mover el área
//   Alt (además)        -> pasos más grandes
//   Ctrl + R            -> reset
//   Ctrl + S            -> guardar (también guarda al salir)
//   Ctrl + F            -> invertir imagen vertical (si en tu PC sale dada vuelta)
//
// Línea de comandos (pisa lo guardado):
//   MiJuego.exe -areaX 0.9 -areaY 0.85 -offX 0 -offY 0
//   areaX/areaY: 0.1 a 1 (fracción de la pantalla). offX/offY: -1 a 1 (dentro del margen sobrante).
//
// Mouse: los clics de la UI (uGUI con StandaloneInputModule) se corrigen solos.
// Para tu propio código usá ScreenAreaAdjuster.MousePosition en lugar de Input.mousePosition,
// o ScreenAreaAdjuster.ToGameScreen(pos) para convertir cualquier punto de pantalla.

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ScreenAreaAdjuster : MonoBehaviour
{
    [Range(0.1f, 1f)] public float scaleX = 1f;
    [Range(0.1f, 1f)] public float scaleY = 1f;
    [Range(-1f, 1f)] public float offsetX = 0f;
    [Range(-1f, 1f)] public float offsetY = 0f;

    public float step = 0.002f;           // por frame mientras se mantiene la tecla
    public bool enableHotkeys = true;
    public bool invertFlip = false;       // por si la imagen sale invertida en algún driver/API
    public FilterMode filter = FilterMode.Bilinear;

    public static ScreenAreaAdjuster Instance { get; private set; }

    const string Prefix = "ScreenArea_";
    RenderTexture capture;
    Material blitMat;

    // ---------------- Arranque automático, una sola instancia para todas las escenas ----------------
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void AutoCreate()
    {
        if (Instance != null) return;
        var go = new GameObject("ScreenAreaAdjuster");
        go.AddComponent<ScreenAreaAdjuster>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
        ReadCommandLine();

        // Sprites/Default viene incluido por defecto en los builds ("Always Included Shaders").
        var sh = Shader.Find("Sprites/Default");
        if (sh == null) sh = Shader.Find("UI/Default");
        blitMat = new Material(sh) { hideFlags = HideFlags.HideAndDontSave };

        SceneManager.sceneLoaded += (s, m) => HookUIInput();
        StartCoroutine(EndOfFrameLoop());
    }

    void OnApplicationQuit() => Save();

    void OnDestroy()
    {
        if (capture != null) { capture.Release(); Destroy(capture); }
        if (blitMat != null) Destroy(blitMat);
    }

    // ---------------- Geometría del área ----------------
    public bool IsFullScreenArea => scaleX >= 0.9999f && scaleY >= 0.9999f;

    /// Área en fracciones de pantalla (0..1), origen abajo-izquierda.
    public Rect AreaRect
    {
        get
        {
            float w = Mathf.Clamp(scaleX, 0.1f, 1f);
            float h = Mathf.Clamp(scaleY, 0.1f, 1f);
            float x = (1f - w) * 0.5f * (1f + Mathf.Clamp(offsetX, -1f, 1f));
            float y = (1f - h) * 0.5f * (1f + Mathf.Clamp(offsetY, -1f, 1f));
            return new Rect(x, y, w, h);
        }
    }

    /// Convierte una posición real de pantalla (píxeles) a la posición equivalente dentro del juego.
    public static Vector2 ToGameScreen(Vector2 screenPos)
    {
        if (Instance == null || Instance.IsFullScreenArea) return screenPos;
        Rect a = Instance.AreaRect;
        float sw = Screen.width, sh = Screen.height;
        return new Vector2(
            (screenPos.x - a.x * sw) / a.width,
            (screenPos.y - a.y * sh) / a.height);
    }

    /// Posición del mouse ya corregida (usar en lugar de Input.mousePosition).
    public static Vector3 MousePosition
    {
        get
        {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            Vector2 p = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
            Vector2 p = Input.mousePosition;
#endif
            return ToGameScreen(p);
        }
    }

    // ---------------- Render: captura el frame final y lo redibuja achicado ----------------
    IEnumerator EndOfFrameLoop()
    {
        var wait = new WaitForEndOfFrame();
        while (true)
        {
            yield return wait;
            if (IsFullScreenArea || blitMat == null) continue;

            EnsureCaptureRT();
            ScreenCapture.CaptureScreenshotIntoRenderTexture(capture);

            bool flip = SystemInfo.graphicsUVStartsAtTop ^ invertFlip;
            float v0 = flip ? 1f : 0f, v1 = flip ? 0f : 1f;
            Rect a = AreaRect;

            RenderTexture.active = null; // backbuffer
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Clear(true, true, Color.black);

            blitMat.mainTexture = capture;
            blitMat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.Color(Color.white);
            GL.TexCoord2(0, v0); GL.Vertex3(a.xMin, a.yMin, 0);
            GL.TexCoord2(0, v1); GL.Vertex3(a.xMin, a.yMax, 0);
            GL.TexCoord2(1, v1); GL.Vertex3(a.xMax, a.yMax, 0);
            GL.TexCoord2(1, v0); GL.Vertex3(a.xMax, a.yMin, 0);
            GL.End();
            GL.PopMatrix();
        }
    }

    void EnsureCaptureRT()
    {
        int w = Screen.width, h = Screen.height;
        if (capture != null && capture.width == w && capture.height == h) { capture.filterMode = filter; return; }
        if (capture != null) { capture.Release(); Destroy(capture); }
        capture = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32)
        {
            filterMode = filter,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };
        capture.Create();
    }

    // ---------------- Input de UI (uGUI) corregido ----------------
    void HookUIInput()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        foreach (var module in FindAll<BaseInputModule>())
        {
            if (module.inputOverride is RemappedInput) continue;
            var ri = module.GetComponent<RemappedInput>();
            if (ri == null) ri = module.gameObject.AddComponent<RemappedInput>();
            module.inputOverride = ri;
        }
#endif
    }

    static T[] FindAll<T>() where T : Object
    {
#if UNITY_2023_1_OR_NEWER
        return FindObjectsByType<T>(FindObjectsSortMode.None);
#else
        return FindObjectsOfType<T>();
#endif
    }

    float hookTimer;
    void Update()
    {
        // Revisa cada tanto por EventSystems creados después de cargar la escena.
        hookTimer -= Time.unscaledDeltaTime;
        if (hookTimer <= 0f) { hookTimer = 1f; HookUIInput(); }

        if (enableHotkeys) HandleHotkeys();
    }

    // ---------------- Teclado ----------------
    void HandleHotkeys()
    {
        bool ctrl = Key("ctrl"), shift = Key("shift"), alt = Key("alt");
        float s = step * (alt ? 5f : 1f);

        if (ctrl && !shift)
        {
            if (Key("left"))  scaleX -= s;
            if (Key("right")) scaleX += s;
            if (Key("down"))  scaleY -= s;
            if (Key("up"))    scaleY += s;
            if (KeyDown("r")) { scaleX = scaleY = 1f; offsetX = offsetY = 0f; }
            if (KeyDown("s")) Save();
            if (KeyDown("f")) invertFlip = !invertFlip;
        }
        else if (shift && !ctrl)
        {
            float o = s * 4f;
            if (Key("left"))  offsetX -= o;
            if (Key("right")) offsetX += o;
            if (Key("down"))  offsetY -= o;
            if (Key("up"))    offsetY += o;
        }

        scaleX = Mathf.Clamp(scaleX, 0.1f, 1f);
        scaleY = Mathf.Clamp(scaleY, 0.1f, 1f);
        offsetX = Mathf.Clamp(offsetX, -1f, 1f);
        offsetY = Mathf.Clamp(offsetY, -1f, 1f);
    }

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
    static bool Key(string k)
    {
        var kb = Keyboard.current; if (kb == null) return false;
        switch (k)
        {
            case "ctrl":  return kb.ctrlKey.isPressed;
            case "shift": return kb.shiftKey.isPressed;
            case "alt":   return kb.altKey.isPressed;
            case "left":  return kb.leftArrowKey.isPressed;
            case "right": return kb.rightArrowKey.isPressed;
            case "up":    return kb.upArrowKey.isPressed;
            case "down":  return kb.downArrowKey.isPressed;
        }
        return false;
    }
    static bool KeyDown(string k)
    {
        var kb = Keyboard.current; if (kb == null) return false;
        switch (k)
        {
            case "r": return kb.rKey.wasPressedThisFrame;
            case "s": return kb.sKey.wasPressedThisFrame;
            case "f": return kb.fKey.wasPressedThisFrame;
        }
        return false;
    }
#else
    static bool Key(string k)
    {
        switch (k)
        {
            case "ctrl":  return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            case "shift": return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            case "alt":   return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            case "left":  return Input.GetKey(KeyCode.LeftArrow);
            case "right": return Input.GetKey(KeyCode.RightArrow);
            case "up":    return Input.GetKey(KeyCode.UpArrow);
            case "down":  return Input.GetKey(KeyCode.DownArrow);
        }
        return false;
    }
    static bool KeyDown(string k)
    {
        switch (k)
        {
            case "r": return Input.GetKeyDown(KeyCode.R);
            case "s": return Input.GetKeyDown(KeyCode.S);
            case "f": return Input.GetKeyDown(KeyCode.F);
        }
        return false;
    }
#endif

    // ---------------- Persistencia ----------------
    void Load()
    {
        scaleX     = PlayerPrefs.GetFloat(Prefix + "scaleX", scaleX);
        scaleY     = PlayerPrefs.GetFloat(Prefix + "scaleY", scaleY);
        offsetX    = PlayerPrefs.GetFloat(Prefix + "offsetX", offsetX);
        offsetY    = PlayerPrefs.GetFloat(Prefix + "offsetY", offsetY);
        invertFlip = PlayerPrefs.GetInt(Prefix + "invertFlip", invertFlip ? 1 : 0) == 1;
    }

    void Save()
    {
        PlayerPrefs.SetFloat(Prefix + "scaleX", scaleX);
        PlayerPrefs.SetFloat(Prefix + "scaleY", scaleY);
        PlayerPrefs.SetFloat(Prefix + "offsetX", offsetX);
        PlayerPrefs.SetFloat(Prefix + "offsetY", offsetY);
        PlayerPrefs.SetInt(Prefix + "invertFlip", invertFlip ? 1 : 0);
        PlayerPrefs.Save();
    }

    void ReadCommandLine()
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (!float.TryParse(args[i + 1], System.Globalization.NumberStyles.Float,
                                System.Globalization.CultureInfo.InvariantCulture, out float v)) continue;
            switch (args[i].ToLowerInvariant())
            {
                case "-areax": scaleX = v; break;
                case "-areay": scaleY = v; break;
                case "-offx":  offsetX = v; break;
                case "-offy":  offsetY = v; break;
            }
        }
    }
}

#if ENABLE_LEGACY_INPUT_MANAGER
/// Reemplaza la posición del mouse/touch que ve el EventSystem por la posición corregida.
public class RemappedInput : BaseInput
{
    public override Vector2 mousePosition => ScreenAreaAdjuster.ToGameScreen(base.mousePosition);

    public override Touch GetTouch(int index)
    {
        Touch t = base.GetTouch(index);
        t.position = ScreenAreaAdjuster.ToGameScreen(t.position);
        return t;
    }
}
#endif