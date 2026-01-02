using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;

[Serializable]
public class dataToSave {
    public string playerName;
    public int totalCoins;
    public int crrLevel;
    public int highScore;
}

public class DataSaver : MonoBehaviour
{
    public dataToSave dts;
    public string userId;
    DatabaseReference dbRef;

    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    } 

    public void SaveDataFn () {
        string json = JsonUtility.ToJson(dts);
        dbRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }


    public void LoadDataFn () {}
}
