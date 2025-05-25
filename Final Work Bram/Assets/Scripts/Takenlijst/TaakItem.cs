using UnityEngine;
using UnityEngine.UI;

public class TaakItem : MonoBehaviour
{
    public Text taakText;          // Sleep hier de Text component naartoe in Inspector
    public Button verwijderKnop;   // Sleep hier de Button naartoe

    private void Start()
    {
        verwijderKnop.onClick.AddListener(VerwijderTaak);
    }

    public void StelIn(string taak)
    {
        taakText.text = taak;
    }

    void VerwijderTaak()
    {
        Destroy(gameObject);
    }
}
