using System;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions; // Noodzakelijk voor ContinueWithOnMainThread

[Serializable]
public class dataToSave
{
    public string playerName;
    public int totalCoins;
    public int crrLevel;
    public int highScore;
}

public class DataSaver : MonoBehaviour
{
    public dataToSave dts;
    public string userId;

    private FirebaseFirestore db;

    private void Awake()
    {
        // Firestore instantie ophalen
        db = FirebaseFirestore.DefaultInstance;
    }

    /// <summary>
    /// Sla de data van deze gebruiker op Firestore op
    /// </summary>
    public void SaveDataFn()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("UserId is leeg! Vul een UserId in.");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(userId);

        docRef.SetAsync(dts).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log("✅ Data opgeslagen in Firestore!");
            else
                Debug.LogError("❌ Fout bij opslaan: " + task.Exception);
        });
    }

    /// <summary>
    /// Laad de data van deze gebruiker van Firestore
    /// </summary>
    public void LoadDataFn()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("UserId is leeg! Vul een UserId in.");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(userId);

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    dataToSave loadedData = snapshot.ConvertTo<dataToSave>();
                    dts = loadedData; // update lokaal object
                    Debug.Log($"✅ Data geladen: {loadedData.playerName}, Coins: {loadedData.totalCoins}");
                }
                else
                {
                    Debug.Log("⚠️ Geen data gevonden voor deze gebruiker.");
                }
            }
            else
            {
                Debug.LogError("❌ Fout bij laden: " + task.Exception);
            }
        });
    }
}
