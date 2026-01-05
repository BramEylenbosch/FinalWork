using UnityEngine;
using TMPro;
using Firebase.Firestore;
using UnityEngine.UI; // voor Button
using System.Collections.Generic;

public class GebruikerKoppelMantelzorger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject koppelPanel;
    [SerializeField] private Button openKoppelPanelKnop; // Open-knop

    private void Awake()
    {
        // Panel standaard uit
        if (koppelPanel != null)
            koppelPanel.SetActive(true);
    }

    private void Start()
    {
        // Listener toevoegen
        if (openKoppelPanelKnop != null)
            openKoppelPanelKnop.onClick.AddListener(OpenKoppelPanel);
    }

    // Open panel
    public void OpenKoppelPanel()
    {
        if (koppelPanel != null)
            koppelPanel.SetActive(true);

        if (openKoppelPanelKnop != null)
            openKoppelPanelKnop.gameObject.SetActive(false); // knop verbergen
    }

    // Sluit panel
    public void SluitKoppelPanel()
    {
        if (koppelPanel != null)
            koppelPanel.SetActive(false);

        if (openKoppelPanelKnop != null)
            openKoppelPanelKnop.gameObject.SetActive(true); // knop weer zichtbaar
    }

    public async void BevestigCode()
    {
        string ingevoerdeCode = codeInputField.text.Trim();

        if (string.IsNullOrEmpty(ingevoerdeCode))
        {
            statusText.text = "Voer een geldige code in!";
            return;
        }

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        var mantelzorgerDoc = await db.Collection("gebruikers")
                                      .Document(ingevoerdeCode)
                                      .GetSnapshotAsync();

        if (!mantelzorgerDoc.Exists)
        {
            statusText.text = "Deze code bestaat niet!";
            return;
        }

        UserContext.CaretakerId = ingevoerdeCode;
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
        Debug.Log("Gebruiker succesvol gekoppeld aan mantelzorger: " + ingevoerdeCode);

        SluitKoppelPanel(); // sluit panel en toont de knop terug
    }
}
