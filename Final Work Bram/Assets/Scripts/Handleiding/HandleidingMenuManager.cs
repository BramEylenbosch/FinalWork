using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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


    void Start()
    {

        viewer.manager = this;

        handleidingen = DataOpslagSystem.LaadHandleidingen();

        foreach (var h in handleidingen)
            MaakKnopVoorHandleiding(h);

        // Knoppen koppelen
        bevestigKnop.onClick.AddListener(BevestigNieuweHandleiding);
        annuleerKnop.onClick.AddListener(() => naamInputPanel.SetActive(false));

        if (handleidingen.Count == 0)
        {
            VoegHandleidingToe("Koffiemachine");
            VoegHandleidingToe("Microgolf");
        }


        // Popup standaard verbergen
        naamInputPanel.SetActive(false);
        
    }

    public void VoegHandleidingToe(string naam)
    {
        var nieuwe = new HandleidingData(naam);
        handleidingen.Add(nieuwe);

        MaakKnopVoorHandleiding(nieuwe);

        DataOpslagSystem.SlaHandleidingenOp(handleidingen);
    }

    void OpenHandleiding(HandleidingData data)
    {
        viewer.ToonHandleiding(data);

        if (handleidingListPanel != null)
            handleidingListPanel.SetActive(false);
    }

    // --- NIEUW ---
    public void StartNieuweHandleiding()
    {
        naamInputField.text = "";
        naamInputPanel.SetActive(true); // popup tonen
    }

    private void BevestigNieuweHandleiding()
    {
        string naam = naamInputField.text.Trim();
        if (!string.IsNullOrEmpty(naam))
        {
            VoegHandleidingToe(naam);
        }
        naamInputPanel.SetActive(false);
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

    public void VerwijderHandleiding(HandleidingData handleiding)
    {
        if (handleiding == null) return;

        // Verwijder uit de lijst
        handleidingen.Remove(handleiding);

        // Opslaan
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
