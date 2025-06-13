using UnityEngine;
using UnityEngine.UI;

public class TaakItem : MonoBehaviour
{
    public Text taakText;    
    public Button verwijderKnop; 

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
