using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;

public class GebruikerInitializer : MonoBehaviour
{
    private const string GEBRUIKER_ID_KEY = "gebruikerId";

    private void Awake()
    {
        if (!PlayerPrefs.HasKey(GEBRUIKER_ID_KEY))
        {
            MaakNieuweGebruiker();
        }
        else
        {
            Debug.Log("Bestaande gebruikerId: " + PlayerPrefs.GetString(GEBRUIKER_ID_KEY));
        }
    }

    private void MaakNieuweGebruiker()
    {
        string gebruikerId = System.Guid.NewGuid().ToString();
        PlayerPrefs.SetString(GEBRUIKER_ID_KEY, gebruikerId);
        PlayerPrefs.Save();

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "mantelzorgerId", "" }
        };

        db.Collection("gebruikers")
          .Document(gebruikerId)
          .SetAsync(data);

        Debug.Log("Nieuwe gebruiker aangemaakt: " + gebruikerId);
    }
}
