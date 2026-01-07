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

        vorigeKnop.onClick.AddListener(Vorige);
        volgendeKnop.onClick.AddListener(Volgende);
        sluitKnop.onClick.AddListener(SluitViewer);


        gameObject.SetActive(false);
    }


    public void ToonHandleiding(HandleidingData data)
    {
        huidigeHandleiding = data;
        huidigeIndex = 0;
        gameObject.SetActive(true);

        ToonPagina();
    }


    private void ToonPagina()
    {
        if (huidigeHandleiding == null || huidigeHandleiding.fotos.Count == 0)
        {
            paginaImage.sprite = null;
            paginaImage.color = Color.gray; 
            vorigeKnop.interactable = false;
            volgendeKnop.interactable = false;
            return;
        }

        paginaImage.sprite = huidigeHandleiding.fotos[huidigeIndex];
        paginaImage.color = Color.white;

        vorigeKnop.interactable = huidigeIndex > 0;
        volgendeKnop.interactable = huidigeIndex < huidigeHandleiding.fotos.Count - 1;
    }


    public void Volgende()
    {
        if (huidigeHandleiding == null) return;
        if (huidigeIndex < huidigeHandleiding.fotos.Count - 1)
        {
            huidigeIndex++;
            ToonPagina();
        }
    }


    public void Vorige()
    {
        if (huidigeHandleiding == null) return;
        if (huidigeIndex > 0)
        {
            huidigeIndex--;
            ToonPagina();
        }
    }


    public void SluitViewer()
    {
        gameObject.SetActive(false);

        if (handleidingListPanel != null)
            handleidingListPanel.SetActive(true);
    }



    public HandleidingData HuidigeHandleiding
    {
        get { return huidigeHandleiding; }
    }
}
