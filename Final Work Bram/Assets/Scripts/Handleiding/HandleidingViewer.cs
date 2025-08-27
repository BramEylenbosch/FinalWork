using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandleidingViewer : MonoBehaviour
{
    [Header("UI")]
    public Image paginaImage;
    public Button vorigeKnop;
    public Button volgendeKnop;
    public Button fotoToevoegenKnop;
    public Button sluitKnop;

    [HideInInspector] public HandleidingManager manager;

    private HandleidingData huidigeHandleiding;
    private int huidigeIndex = 0;

    void Start()
    {
        // Koppel de knoppen
        vorigeKnop.onClick.AddListener(Vorige);
        volgendeKnop.onClick.AddListener(Volgende);
        fotoToevoegenKnop.onClick.AddListener(NeemFoto);
        sluitKnop.onClick.AddListener(SluitViewer);

        gameObject.SetActive(false); // standaard verbergen
    }

    // Toon een specifieke handleiding
    public void ToonHandleiding(HandleidingData data)
    {
        huidigeHandleiding = data;
        huidigeIndex = 0;
        Debug.Log("ToonHandleiding aangeroepen, zet viewer actief: " + gameObject.name);
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

    // Foto maken met NativeCamera
    public void NeemFoto()
    {
        NativeCamera.TakePicture((path) =>
        {
            if (path != null)
            {
                Debug.Log("Foto opgeslagen op: " + path);

                // Laad de foto als Texture2D
                Texture2D texture = NativeCamera.LoadImageAtPath(path, 1024);
                if (texture == null)
                {
                    Debug.LogError("Kon foto niet laden!");
                    return;
                }

                // Maak er een Sprite van
                Sprite nieuweSprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));

                // Voeg toe aan handleiding
                FotoToevoegen(nieuweSprite);
            }
        }, maxSize: 1024);
    }


    // Voeg een foto toe aan de huidige handleiding
    public void FotoToevoegen(Sprite nieuweFoto)
    {
        if (huidigeHandleiding == null) return;

        huidigeHandleiding.fotos.Add(nieuweFoto);
        huidigeIndex = huidigeHandleiding.fotos.Count - 1; // laatste foto tonen
        ToonPagina();
    }

    // Viewer sluiten
    public void SluitViewer()
    {
        gameObject.SetActive(false);

        if (manager != null && manager.handleidingListPanel != null)
            manager.handleidingListPanel.SetActive(true);
    }
}
