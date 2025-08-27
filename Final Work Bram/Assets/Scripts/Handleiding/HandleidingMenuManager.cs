using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandleidingManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform handleidingListParent;    // Content van ScrollView
    public GameObject handleidingButtonPrefab; // Prefab met knop
    public GameObject handleidingListPanel;    // <-- zet hier je "HandleidingList" object in de Inspector

    public HandleidingViewer viewer; // verwijzing naar de viewer

    private List<HandleidingData> handleidingen = new List<HandleidingData>();

    void Start()
    {
        // koppel viewer terug naar manager
        viewer.manager = this;

        // Testvoorbeelden
        VoegHandleidingToe("Koffiemachine");
        VoegHandleidingToe("Microgolf");
    }

    public void VoegHandleidingToe(string naam)
    {
        var nieuwe = new HandleidingData(naam);
        handleidingen.Add(nieuwe);

        // Maak een knop
        GameObject knop = Instantiate(handleidingButtonPrefab, handleidingListParent);
        knop.GetComponentInChildren<TextMeshProUGUI>().text = naam;
        knop.GetComponent<Button>().onClick.AddListener(() => OpenHandleiding(nieuwe));
    }

    void OpenHandleiding(HandleidingData data)
    {
        Debug.Log("OpenHandleiding - viewer object = " + viewer.gameObject.name);
        viewer.ToonHandleiding(data);

        if (handleidingListPanel != null)
            handleidingListPanel.SetActive(false);
    }

}
