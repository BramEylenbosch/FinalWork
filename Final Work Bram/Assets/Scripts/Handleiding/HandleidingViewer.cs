using UnityEngine;
using UnityEngine.UI;

public class HandleidingViewer : MonoBehaviour
{
    public Image paginaImage;
    public Button vorigeKnop;
    public Button volgendeKnop;

    private Sprite[] paginas;
    private int huidigePagina = 0;

    void Start()
    {
        paginas = Resources.LoadAll<Sprite>("Handleiding/Microgolf");

        if (paginas.Length == 0)
        {
            Debug.LogError("Geen pagina's gevonden in Resources/Handleiding!");
            return;
        }

        ToonPagina(0);

        vorigeKnop.onClick.AddListener(ToonVorige);
        volgendeKnop.onClick.AddListener(ToonVolgende);
    }

    void ToonPagina(int index)
    {
        paginaImage.sprite = paginas[index];
        huidigePagina = index;

        vorigeKnop.interactable = (huidigePagina > 0);
        volgendeKnop.interactable = (huidigePagina < paginas.Length - 1);
    }

    void ToonVorige() => ToonPagina(huidigePagina - 1);
    void ToonVolgende() => ToonPagina(huidigePagina + 1);
}
