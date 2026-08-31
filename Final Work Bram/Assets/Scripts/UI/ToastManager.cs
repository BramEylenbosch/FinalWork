using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance { get; private set; }

    private Canvas _canvas;
    private RectTransform _toastRoot;
    private Coroutine _activeToast;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;
        var go = new GameObject("ToastManager");
        go.AddComponent<ToastManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildCanvas();
    }

    private void BuildCanvas()
    {
        _canvas = gameObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 9999;

        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();

        var rootGO = new GameObject("ToastRoot");
        rootGO.transform.SetParent(transform, false);
        _toastRoot = rootGO.AddComponent<RectTransform>();
        _toastRoot.anchorMin = new Vector2(0.5f, 0f);
        _toastRoot.anchorMax = new Vector2(0.5f, 0f);
        _toastRoot.pivot = new Vector2(0.5f, 0f);
        _toastRoot.anchoredPosition = new Vector2(0f, 120f);
        _toastRoot.sizeDelta = new Vector2(900f, 120f);
    }

    public static void Show(string message, ToastType type = ToastType.Info, float duration = 3f)
    {
        if (Instance == null) Bootstrap();
        Instance.ShowInternal(message, type, duration);
    }

    public static void ShowSuccess(string message) => Show(message, ToastType.Success);
    public static void ShowError(string message) => Show(message, ToastType.Error);
    public static void ShowWarning(string message) => Show(message, ToastType.Warning);

    private void ShowInternal(string message, ToastType type, float duration)
    {
        if (_activeToast != null)
            StopCoroutine(_activeToast);

        foreach (Transform child in _toastRoot)
            Destroy(child.gameObject);

        var toastGO = new GameObject("Toast");
        toastGO.transform.SetParent(_toastRoot, false);

        var rt = toastGO.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var bg = toastGO.AddComponent<Image>();
        bg.color = type switch
        {
            ToastType.Success => UITheme.Success,
            ToastType.Error => UITheme.Error,
            ToastType.Warning => UITheme.Warning,
            _ => UITheme.PrimaryDark
        };

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(toastGO.transform, false);
        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(24f, 12f);
        textRT.offsetMax = new Vector2(-24f, -12f);

        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = message;
        tmp.fontSize = 28f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.enableWordWrapping = true;

        _activeToast = StartCoroutine(HideAfter(duration, toastGO));
    }

    private IEnumerator HideAfter(float duration, GameObject toastGO)
    {
        yield return new WaitForSeconds(duration);
        if (toastGO != null)
            Destroy(toastGO);
        _activeToast = null;
    }
}

public enum ToastType
{
    Info,
    Success,
    Error,
    Warning
}
