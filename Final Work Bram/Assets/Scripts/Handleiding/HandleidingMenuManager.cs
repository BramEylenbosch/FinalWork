using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HandleidingManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform handleidingListParent;
    public GameObject handleidingButtonPrefab;
    public GameObject handleidingListPanel;
    public HandleidingViewer viewer;

    [Header("Naam toevoegen UI")]
    public GameObject naamInputPanel;          // popup panel met inputveld en OK/Annuleer
    public TMP_InputField naamInputField;      // tekstveld
    public Button bevestigKnop;
    public Button annuleerKnop;

    private List<HandleidingData> handleidingen = new List<HandleidingData>();

    public enum GebruikerType { Mantelzorger, Gebruiker }
    public GebruikerType gebruikerType;
    public int maxHandleidingen = 8; // stel hier de limiet in


async void Start()
{
    // Laad handleidingen van de huidige gebruiker
    handleidingen = await FirestoreHandleidingService.Instance.LaadHandleidingen();

    foreach (var h in handleidingen)
    {
        // Voor elke foto-URL, download als Sprite
        foreach (string url in h.fotoUrls)
        {
            StartCoroutine(DownloadSprite(url, h));
        }

        MaakKnopVoorHandleiding(h);
    }

    // Toon lijstpanel
    if (handleidingListPanel != null)
        handleidingListPanel.SetActive(true);
}
    IEnumerator DownloadSprite(string url, HandleidingData handleiding)
    {
        using var www = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.LogError("[DownloadSprite] Fout: " + www.error);
            yield break;
        }

        Texture2D tex = UnityEngine.Networking.DownloadHandlerTexture.GetContent(www);
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        handleiding.fotos.Add(sprite);
    }

public async void VoegHandleidingToe(string naam)
{
    if (handleidingen.Count >= maxHandleidingen)
    {
        Debug.Log("Maximum aantal handleidingen bereikt!");
        return;
    }

    string id = System.Guid.NewGuid().ToString();

    var nieuwe = new HandleidingData(naam)
    {
        id = id
    };

    handleidingen.Add(nieuwe);
    MaakKnopVoorHandleiding(nieuwe);

    // Opslaan lokaal
    DataOpslagSystem.SlaHandleidingenOp(handleidingen);

    // Opslaan in Firestore
    await FirestoreHandleidingService.Instance.VoegHandleidingToe(id, naam);
}


private void OpenHandleiding(HandleidingData data)
{
    // Zorg dat de viewer altijd een manager heeft
    viewer.manager = this;

    // Open handleiding (1 argument)
    viewer.ToonHandleiding(data);

    handleidingListPanel.SetActive(false);
}



    // --- NIEUW ---
public void StartNieuweHandleiding()
{
    naamInputField.text = "";
    naamInputPanel.SetActive(true);

    if (handleidingListPanel != null)
        handleidingListPanel.SetActive(false);

    // Bevestigknop koppelen aan nieuwe handleiding
    bevestigKnop.onClick.RemoveAllListeners();
    bevestigKnop.onClick.AddListener(BevestigNieuweHandleiding);
    annuleerKnop.onClick.RemoveAllListeners();
    annuleerKnop.onClick.AddListener(() => 
    {
        naamInputPanel.SetActive(false);
        if (handleidingListPanel != null)
            handleidingListPanel.SetActive(true);
    });
}


private void BevestigNieuweHandleiding()
{
    string naam = naamInputField.text.Trim();
    if (!string.IsNullOrEmpty(naam))
        VoegHandleidingToe(naam);

    naamInputPanel.SetActive(false);

    if (handleidingListPanel != null)
        handleidingListPanel.SetActive(true);
}

    

    public List<HandleidingData> GetHandleidingen()
    {
        return handleidingen;
    }
    private void MaakKnopVoorHandleiding(HandleidingData h)
    {
        GameObject knop = Instantiate(handleidingButtonPrefab, handleidingListParent);

        var controller = knop.GetComponent<HandleidingButtonController>();
        if (controller != null)
            controller.Setup(h, this);  // ⚡ koppelt naam + verwijderknop automatisch

        // Hoofdknop openen handleiding
        Button mainButton = knop.GetComponent<Button>();
        if (mainButton != null)
            mainButton.onClick.AddListener(() => OpenHandleiding(h));
    }

public async void VerwijderHandleiding(HandleidingData handleiding)
{
    if (handleiding == null) return;

    // Verwijder uit Firestore
    await FirestoreHandleidingService.Instance.VerwijderHandleiding(handleiding.id);

    // Verwijder uit de lijst
    handleidingen.Remove(handleiding);

    // Opslaan lokaal
    DataOpslagSystem.SlaHandleidingenOp(handleidingen);

    // Verwijder bijbehorende knop uit de UI
    foreach (Transform child in handleidingListParent)
    {
        TextMeshProUGUI txt = child.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null && txt.text == handleiding.naam)
        {
            GameObject.Destroy(child.gameObject);
            break;
        }
    }

    // Als de viewer deze handleiding open heeft, sluit hem
    if (viewer != null && viewer.HuidigeHandleiding == handleiding)
    {
        viewer.SluitViewer();
    }
}

}
