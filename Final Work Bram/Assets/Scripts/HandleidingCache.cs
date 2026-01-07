using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class HandleidingCache
{
    private static string cacheFile => Path.Combine(Application.persistentDataPath, "handleidingen.json");

    [System.Serializable]
    private class HandleidingListWrapper
    {
        public List<HandleidingDataSerializable> handleidingen;
    }

    [System.Serializable]
    private class HandleidingDataSerializable
    {
        public string id;
        public string naam;
        public List<string> fotoUrls;
        public List<string> lokaleFotoPaden;
    }

    public static void SaveHandleidingen(List<HandleidingData> handleidingen)
    {
        List<HandleidingDataSerializable> serializableList = new List<HandleidingDataSerializable>();
        foreach (var h in handleidingen)
        {
            var s = new HandleidingDataSerializable
            {
                id = h.id,
                naam = h.naam,
                fotoUrls = h.fotoUrls,
                lokaleFotoPaden = new List<string>()
            };

            foreach (var sprite in h.fotos)
            {
                s.lokaleFotoPaden.Add(sprite.name); 
            }

            serializableList.Add(s);
        }

        HandleidingListWrapper wrapper = new HandleidingListWrapper { handleidingen = serializableList };
        string json = JsonUtility.ToJson(wrapper);
        File.WriteAllText(cacheFile, json);
    }


    public static List<HandleidingData> LoadHandleidingen()
    {
        if (!File.Exists(cacheFile)) return new List<HandleidingData>();

        string json = File.ReadAllText(cacheFile);
        HandleidingListWrapper wrapper = JsonUtility.FromJson<HandleidingListWrapper>(json);

        List<HandleidingData> lijst = new List<HandleidingData>();
        foreach (var h in wrapper.handleidingen)
        {
            var hd = new HandleidingData(h.naam)
            {
                id = h.id,
                fotoUrls = h.fotoUrls
            };

            hd.fotos = new List<Sprite>();
            foreach (var pad in h.lokaleFotoPaden)
            {
                string fullPad = Path.Combine(Application.persistentDataPath, pad);
                if (File.Exists(fullPad))
                {
                    byte[] bytes = File.ReadAllBytes(fullPad);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(bytes);
                    hd.fotos.Add(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f)));
                    hd.fotos[hd.fotos.Count - 1].name = pad;
                }
            }

            lijst.Add(hd);
        }

        return lijst;
    }

    public static async Task<string> DownloadEnSlaFotoOpLocaal(string fotoUrl, string bestandNaam)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(fotoUrl))
        {
            var op = www.SendWebRequest();
            while (!op.isDone) await Task.Yield();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("[HandleidingCache] Foto downloaden mislukt: " + www.error);
                return null;
            }

            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            byte[] bytes = tex.EncodeToPNG();
            string pad = Path.Combine(Application.persistentDataPath, bestandNaam + ".png");
            File.WriteAllBytes(pad, bytes);
            return pad;
        }
    }
}
