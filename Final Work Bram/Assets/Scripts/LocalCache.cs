using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class LocalCache
{
    private static string takenFile => Path.Combine(Application.persistentDataPath, "taken.json");

    [System.Serializable]
    private class TaakListWrapper
    {
        public List<Taak> taken;
    }

    public static void SaveTaken(List<Taak> taken)
    {
        string json = JsonUtility.ToJson(new TaakListWrapper { taken = taken });
        File.WriteAllText(takenFile, json);
    }

    public static List<Taak> LoadTaken()
    {
        if (!File.Exists(takenFile)) return new List<Taak>();
        string json = File.ReadAllText(takenFile);
        return JsonUtility.FromJson<TaakListWrapper>(json).taken ?? new List<Taak>();
    }
}
