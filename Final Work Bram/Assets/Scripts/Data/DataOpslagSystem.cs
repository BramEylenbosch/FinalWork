using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataOpslagSystem
{
    private static string bestandPad => Path.Combine(Application.persistentDataPath, "handleidingen.json");

    public static void SlaHandleidingenOp(List<HandleidingData> handleidingen)
    {
        foreach (var h in handleidingen)
        {
            if (h.fotoUrls == null)
                h.fotoUrls = new List<string>();
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


        foreach (var h in wrapper.handleidingen)
        {
            if (string.IsNullOrEmpty(h.id))
                h.id = System.Guid.NewGuid().ToString();
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
    #endregion

    
}
