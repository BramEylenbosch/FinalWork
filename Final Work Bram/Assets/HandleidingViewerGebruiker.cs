using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandleidingViewerGebruiker : MonoBehaviour
{
    [Header("UI")]
    public Image paginaImage;
    public Button vorigeKnop;
    public Button volgendeKnop;
    public Button sluitKnop;

    private HandleidingData huidigeHandleiding;
    private int huidigeIndex = 0;
    public GameObject handleidingListPanel; 


    void Start()
    {
        // Koppel de knoppen
        vorigeKnop.onClick.AddListener(Vorige);
        volgendeKnop.onClick.AddListener(Volgende);
        sluitKnop.onClick.AddListener(SluitViewer);

        // Zet standaard verborgen
        gameObject.SetActive(false);
    }

    // Toon een specifieke handleiding
    public void ToonHandleiding(HandleidingData data)
    {
        huidigeHandleiding = data;
        huidigeIndex = 0;
        gameObject.SetActive(true);

        ToonPagina();
    }

    // Toon de huidige pagina
    private void ToonPagina()
    {
        if (huidigeHandleiding == null || huidigeHandleiding.fotos.Count == 0)
        {
            paginaImage.sprite = null;
            paginaImage.color = Color.gray; // lege placeholder
            vorigeKnop.interactable = false;
            volgendeKnop.interactable = false;
            return;
        }

        paginaImage.sprite = huidigeHandleiding.fotos[huidigeIndex];
        paginaImage.color = Color.white;

        vorigeKnop.interactable = huidigeIndex > 0;
        volgendeKnop.interactable = huidigeIndex < huidigeHandleiding.fotos.Count - 1;
    }

    // Ga naar volgende pagina
    public void Volgende()
    {
        if (huidigeHandleiding == null) return;
        if (huidigeIndex < huidigeHandleiding.fotos.Count - 1)
        {
            huidigeIndex++;
            ToonPagina();
        }
    }

    // Ga naar vorige pagina
    public void Vorige()
    {
        if (huidigeHandleiding == null) return;
        if (huidigeIndex > 0)
        {
            huidigeIndex--;
            ToonPagina();
        }
    }

    // Viewer sluiten
    public void SluitViewer()
    {
        gameObject.SetActive(false);

        if (handleidingListPanel != null)
            handleidingListPanel.SetActive(true);
    }


    // Getter voor huidige handleiding
    public HandleidingData HuidigeHandleiding
    {
        get { return huidigeHandleiding; }
    }
}
