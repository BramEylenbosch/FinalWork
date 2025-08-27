using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandleidingManagerGebruiker : MonoBehaviour
{
    [Header("UI References")]
    public Transform handleidingListParent;
    public GameObject handleidingButtonPrefab;
    public GameObject handleidingListPanel;

    private List<HandleidingData> handleidingen = new List<HandleidingData>();

    public HandleidingViewerGebruiker viewer;

    void Start()
    {


        // Laad opgeslagen handleidingen
        handleidingen = DataOpslagSystem.LaadHandleidingen();

        // Maak knop voor elke handleiding
        foreach (var h in handleidingen)
            MaakKnopVoorHandleiding(h);

        // Zorg dat de lijst zichtbaar is
        if (handleidingListPanel != null)
            handleidingListPanel.SetActive(true);
    }

    private void MaakKnopVoorHandleiding(HandleidingData h)
    {
        GameObject knop = Instantiate(handleidingButtonPrefab, handleidingListParent);

        // Zet alleen de naam op de knop
        TextMeshProUGUI naamText = knop.GetComponentInChildren<TextMeshProUGUI>();
        if (naamText != null)
            naamText.text = h.naam;

        // Koppel de knop aan openen van handleiding
        Button mainButton = knop.GetComponent<Button>();
        if (mainButton != null)
            mainButton.onClick.AddListener(() => OpenHandleiding(h));

        // Verwijder alle verwijderknoppen uit prefab (indien aanwezig)
        HandleidingButtonController controller = knop.GetComponent<HandleidingButtonController>();
        if (controller != null && controller.verwijderKnop != null)
            controller.verwijderKnop.gameObject.SetActive(false);
    }

    private void OpenHandleiding(HandleidingData data)
    {
        if (viewer != null)
        {
            viewer.ToonHandleiding(data);

            if (handleidingListPanel != null)
                handleidingListPanel.SetActive(false);
        }
    }
    
}
