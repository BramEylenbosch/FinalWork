using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class UIConfirmDialog
{
    private static GameObject _overlay;

    public static void Show(string title, string message, Action onConfirm, Action onCancel = null)
    {
        Hide();

        _overlay = new GameObject("ConfirmDialog");
        var canvas = _overlay.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000;

        var scaler = _overlay.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        _overlay.AddComponent<GraphicRaycaster>();

        var dimmer = CreatePanel(_overlay.transform, "Dimmer", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        dimmer.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);

        var panel = CreatePanel(_overlay.transform, "Panel",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-420f, -220f), new Vector2(420f, 220f));
        panel.GetComponent<Image>().color = UITheme.Panel;

        CreateTMP(panel.transform, title, 36f, FontStyles.Bold,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -40f), new Vector2(760f, 60f), TextAlignmentOptions.Center);

        CreateTMP(panel.transform, message, 28f, FontStyles.Normal,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0f, 20f), new Vector2(760f, 180f), TextAlignmentOptions.Center);

        var cancelBtn = CreateButton(panel.transform, "Annuleren",
            new Vector2(0.25f, 0f), new Vector2(0.25f, 0f),
            new Vector2(-160f, 50f), new Vector2(300f, 80f), UITheme.TextMuted);
        cancelBtn.onClick.AddListener(() =>
        {
            Hide();
            onCancel?.Invoke();
        });

        var confirmBtn = CreateButton(panel.transform, "Verwijderen",
            new Vector2(0.75f, 0f), new Vector2(0.75f, 0f),
            new Vector2(-160f, 50f), new Vector2(300f, 80f), UITheme.Error);
        confirmBtn.onClick.AddListener(() =>
        {
            Hide();
            onConfirm?.Invoke();
        });
    }

    public static void Hide()
    {
        if (_overlay != null)
        {
            UnityEngine.Object.Destroy(_overlay);
            _overlay = null;
        }
    }

    private static GameObject CreatePanel(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        go.AddComponent<Image>();
        return go;
    }

    private static TextMeshProUGUI CreateTMP(Transform parent, string text, float fontSize, FontStyles style,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta, TextAlignmentOptions align)
    {
        var go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = align;
        tmp.color = UITheme.TextPrimary;
        tmp.enableWordWrapping = true;
        return tmp;
    }

    private static Button CreateButton(Transform parent, string label,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta, Color bgColor)
    {
        var go = new GameObject(label);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;

        var image = go.AddComponent<Image>();
        image.color = bgColor;

        var button = go.AddComponent<Button>();
        UITheme.StylePrimaryButton(button);

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 26f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return button;
    }
}
