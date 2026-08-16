using UnityEngine;
using UnityEngine.UI;

public static class UITheme
{
    public static readonly Color Primary = new Color(0.769f, 0.906f, 0.941f);
    public static readonly Color PrimaryDark = new Color(0.220f, 0.714f, 1.0f);
    public static readonly Color Background = new Color(0.859f, 0.961f, 0.953f);
    public static readonly Color Panel = new Color(0.847f, 0.988f, 1.0f);
    public static readonly Color TextPrimary = new Color(0.196f, 0.196f, 0.196f);
    public static readonly Color TextMuted = new Color(0.55f, 0.55f, 0.55f);
    public static readonly Color TextOnPrimary = Color.white;
    public static readonly Color Success = new Color(0.18f, 0.72f, 0.45f);
    public static readonly Color Error = new Color(0.85f, 0.25f, 0.25f);
    public static readonly Color Warning = new Color(0.95f, 0.65f, 0.15f);
    public static readonly Color TodayHighlight = new Color(0.220f, 0.714f, 1.0f, 0.35f);

    public static void StylePrimaryButton(Button button)
    {
        if (button == null) return;

        var image = button.GetComponent<Image>();
        if (image != null)
            image.color = Primary;

        var colors = button.colors;
        colors.normalColor = Primary;
        colors.highlightedColor = PrimaryDark;
        colors.pressedColor = new Color(0.65f, 0.82f, 0.88f);
        colors.selectedColor = PrimaryDark;
        button.colors = colors;

        var text = button.GetComponentInChildren<TMPro.TMP_Text>();
        if (text != null)
            text.color = TextPrimary;
    }

    public static void StyleSecondaryButton(Button button)
    {
        if (button == null) return;

        var image = button.GetComponent<Image>();
        if (image != null)
            image.color = Panel;

        var text = button.GetComponentInChildren<TMPro.TMP_Text>();
        if (text != null)
            text.color = TextPrimary;
    }
}
