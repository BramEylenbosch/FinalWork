using UnityEngine;
using TMPro;
using Firebase.Firestore;
using UnityEngine.UI;
using System.Collections.Generic;

public class GebruikerKoppelMantelzorger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject koppelPanel;
    [SerializeField] private Button openKoppelPanelKnop;

    private void Awake()
    {
        if (koppelPanel != null)
            koppelPanel.SetActive(true);
    }

    private void Start()
    {
        if (openKoppelPanelKnop != null)
            openKoppelPanelKnop.onClick.AddListener(OpenKoppelPanel);
    }

    public void OpenKoppelPanel()
    {
        if (koppelPanel != null)
            koppelPanel.SetActive(true);

        if (openKoppelPanelKnop != null)
            openKoppelPanelKnop.gameObject.SetActive(false); 
    }

    public void SluitKoppelPanel()
    {
        if (koppelPanel != null)
            koppelPanel.SetActive(false);

        if (openKoppelPanelKnop != null)
            openKoppelPanelKnop.gameObject.SetActive(true); 
    }

public async void BevestigCode()
{
    string ingevoerdeCode = codeInputField.text.Trim();
    if (string.IsNullOrEmpty(ingevoerdeCode)) return;

    FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

    var mantelzorgerDoc = await db.Collection("gebruikers").Document(ingevoerdeCode).GetSnapshotAsync();
    if (!mantelzorgerDoc.Exists)
    {
        statusText.text = "Deze code bestaat niet!";
        return;
    }

    UserContext.CaretakerId = ingevoerdeCode;
    PlayerPrefs.SetString("caretakerId", ingevoerdeCode);
    PlayerPrefs.Save();

    statusText.text = "Succes! Je bent gekoppeld aan de mantelzorger.";
    SluitKoppelPanel();

    TaaklijstManager taaklijst = FindObjectOfType<TaaklijstManager>();
    if (taaklijst != null)
    {
        await taaklijst.HerlaadTaken(); 
    }
}

}
