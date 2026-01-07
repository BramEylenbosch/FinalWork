using UnityEngine;
using Firebase.Firestore;
using System;
using System.Collections.Generic;

public class GebruikerInitializer : MonoBehaviour
{
    public static GebruikerInitializer Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // optioneel, als je wilt dat dit object blijft tussen scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Laad UserId en CaretakerId
        UserContext.UserId = PlayerPrefs.GetString(UserContext.USER_ID_KEY, "");
        UserContext.CaretakerId = PlayerPrefs.GetString("caretakerId", "");
    }

    public async void MaakNieuweGebruiker()
    {
        if (!string.IsNullOrEmpty(UserContext.UserId))
            return;

        string userId = System.Guid.NewGuid().ToString();
        PlayerPrefs.SetString(UserContext.USER_ID_KEY, userId);
        PlayerPrefs.Save();

        UserContext.UserId = userId;

        Firebase.Firestore.FirebaseFirestore db = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
        var data = new System.Collections.Generic.Dictionary<string, object>
        {
            { "mantelzorgerId", "" }
        };
        await db.Collection("gebruikers").Document(userId).SetAsync(data);
        Debug.Log("Nieuwe gebruiker aangemaakt: " + userId);
    }
}

