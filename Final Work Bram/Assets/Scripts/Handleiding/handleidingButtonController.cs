using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandleidingButtonController : MonoBehaviour
{
    public Button verwijderKnop;
    public TextMeshProUGUI naamText;

    private HandleidingData handleiding;
    private HandleidingManager manager;

    // Wordt aangeroepen na het instantiëren
public void Setup(HandleidingData data, HandleidingManager manager)
{
    Debug.Log("Setup aangeroepen voor: " + data.naam);

    this.handleiding = data;
    this.manager = manager;

    if (naamText != null)
        naamText.text = data.naam;

    if (verwijderKnop != null)
    {
        Debug.Log("Verwijder knop aanwezig");
        verwijderKnop.onClick.RemoveAllListeners();
        verwijderKnop.onClick.AddListener(() =>
        {
            Debug.Log("Verwijder knop geklikt voor: " + handleiding.naam);
            if (manager != null)
                manager.VerwijderHandleiding(handleiding);
        });
    }
}

}
