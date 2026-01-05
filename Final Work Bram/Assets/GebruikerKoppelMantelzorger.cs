using UnityEngine;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System;
 
public class GebruikerKoppelMantelzorger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField codeInputField;  // Sleep hier je InputField
    [SerializeField] private TextMeshProUGUI statusText;     // Sleep hier je TextMeshProUGUI

    public async void BevestigCode()
    {
        string ingevoerdeCode = codeInputField.text.Trim();

        if (string.IsNullOrEmpty(ingevoerdeCode))
        {
            statusText.text = "Voer een geldige code in!";
            Debug.LogWarning("[GebruikerKoppelMantelzorger] Geen code ingevoerd.");
            return;
        }

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        try
        {
            Debug.Log("[GebruikerKoppelMantelzorger] Controleer of mantelzorger bestaat: " + ingevoerdeCode);

            // Controleer of de mantelzorger bestaat
            var mantelzorgerDoc = await db.Collection("gebruikers")
                                          .Document(ingevoerdeCode)
                                          .GetSnapshotAsync();

            if (!mantelzorgerDoc.Exists)
            {
                statusText.text = "Deze code bestaat niet!";
                Debug.LogWarning("[GebruikerKoppelMantelzorger] Mantelzorger niet gevonden: " + ingevoerdeCode);
                return;
            }

            Debug.Log("[GebruikerKoppelMantelzorger] Mantelzorger gevonden, koppel gebruiker...");

            // Update UserContext en Firestore document van de gebruiker
            UserContext.CaretakerId = ingevoerdeCode;

            // Opslaan in PlayerPrefs zodat de code onthouden wordt
            PlayerPrefs.SetString("caretakerId", ingevoerdeCode);
            PlayerPrefs.Save();

            var updates = new Dictionary<string, object>
            {
                { "mantelzorgerId", ingevoerdeCode }
            };

            await db.Collection("gebruikers")
                    .Document(UserContext.UserId)
                    .UpdateAsync(updates);


            statusText.text = "Succes! Je bent gekoppeld aan de mantelzorger.";
            Debug.Log("[GebruikerKoppelMantelzorger] Gebruiker succesvol gekoppeld aan mantelzorger: " + ingevoerdeCode);
        }
        catch (Exception e)
        {
            statusText.text = "Er is iets misgegaan bij het koppelen!";
            Debug.LogError("[GebruikerKoppelMantelzorger] Fout bij koppelen: " + e);
        }
    }
}
