using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataOpslagSystem
{
    private static string bestandPad => Path.Combine(Application.persistentDataPath, "handleidingen.json");

    public static void SlaHandleidingenOp(List<HandleidingData> handleidingen)
    {
        // Converteer sprites naar Base64
        foreach (var h in handleidingen)
        {
            if (h.fotosBase64 == null)
                h.fotosBase64 = new List<string>();

            h.fotosBase64.Clear();

            if (h.fotos != null)
            {
                foreach (var foto in h.fotos)
                {
                    string b64 = SpriteToBase64(foto);
                    if (!string.IsNullOrEmpty(b64))
                        h.fotosBase64.Add(b64);
                }
            }
        }

        string json = JsonUtility.ToJson(new HandleidingDataWrapper(handleidingen), true);
        File.WriteAllText(bestandPad, json);
        Debug.Log("Handleidingen opgeslagen op: " + bestandPad);
    }

    public static List<HandleidingData> LaadHandleidingen()
    {
        if (!File.Exists(bestandPad))
            return new List<HandleidingData>();

        string json = File.ReadAllText(bestandPad);
        if (string.IsNullOrEmpty(json))
            return new List<HandleidingData>();

        HandleidingDataWrapper wrapper = JsonUtility.FromJson<HandleidingDataWrapper>(json);
        if (wrapper == null || wrapper.handleidingen == null)
            return new List<HandleidingData>();

        // Converteer Base64 terug naar Sprite
        foreach (var h in wrapper.handleidingen)
        {
            if (h.fotos == null)
                h.fotos = new List<Sprite>();

            h.fotos.Clear();

            if (h.fotosBase64 != null)
            {
                foreach (var b64 in h.fotosBase64)
                {
                    if (!string.IsNullOrEmpty(b64))
                    {
                        Sprite s = Base64ToSprite(b64);
                        if (s != null)
                            h.fotos.Add(s);
                    }
                }
            }
        }

        return wrapper.handleidingen;
    }

    #region Helpers
    [System.Serializable]
    private class HandleidingDataWrapper
    {
        public List<HandleidingData> handleidingen;
        public HandleidingDataWrapper(List<HandleidingData> handleidingen)
        {
            this.handleidingen = handleidingen;
        }
    }

private static string SpriteToBase64(Sprite sprite)
{
    if (sprite == null || sprite.texture == null)
        return null;

    Texture2D tex = sprite.texture;

    // ⚡ check of texture readable is
    if (!tex.isReadable)
    {
        Debug.LogError("Texture is niet readable: " + sprite.name);
        return null;
    }

    byte[] bytes = tex.EncodeToPNG();
    return System.Convert.ToBase64String(bytes);
}


    public static Sprite Base64ToSprite(string base64)
    {
        if (string.IsNullOrEmpty(base64)) return null;

        byte[] bytes = System.Convert.FromBase64String(base64);
        Texture2D tex = new Texture2D(2, 2);
        if (!tex.LoadImage(bytes))
            return null;

        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
    #endregion
}
