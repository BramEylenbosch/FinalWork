using UnityEngine;
using Firebase.Firestore;
using System;
using System.Collections.Generic;

public class GebruikerInitializer : MonoBehaviour
{
    public static GebruikerInitializer Instance; // Singleton

    private void Awake()
    {
        // Zorg dat UserId en CaretakerId worden geladen
        UserContext.UserId = PlayerPrefs.GetString(UserContext.USER_ID_KEY, "");
        UserContext.CaretakerId = PlayerPrefs.GetString("caretakerId", "");

        Debug.Log($"[GebruikerInitializer] UserId: {UserContext.UserId}, CaretakerId: {UserContext.CaretakerId}");
    }



    // Publieke methode voor aanmaken van nieuwe gebruiker
    public async void MaakNieuweGebruiker()
    {
        if (!string.IsNullOrEmpty(UserContext.UserId))
        {
            Debug.Log("[GebruikerInitializer] UserId bestaat al, geen nieuwe gebruiker aangemaakt.");
            return;
        }

        string userId = Guid.NewGuid().ToString();
        PlayerPrefs.SetString(UserContext.USER_ID_KEY, userId);
        PlayerPrefs.Save();

        UserContext.UserId = userId;

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        var data = new Dictionary<string, object>
        {
            { "mantelzorgerId", "" }
        };

        await db.Collection("gebruikers").Document(userId).SetAsync(data);
        Debug.Log("[GebruikerInitializer] Nieuwe gebruiker aangemaakt: " + userId);
    }
}
