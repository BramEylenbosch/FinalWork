using UnityEngine;
using UnityEngine.UI;

public static class UICanvasSetup
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void FixAllCanvases()
    {
        foreach (var scaler in Object.FindObjectsByType<CanvasScaler>(FindObjectsSortMode.None))
        {
            if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
                scaler.matchWidthOrHeight = 0.5f;
            }
        }
    }
}
