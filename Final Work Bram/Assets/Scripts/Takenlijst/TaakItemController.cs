using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TaakItemController : MonoBehaviour
{
    public TextMeshProUGUI tekstText;
    public TextMeshProUGUI deadlineText;
    public Button verwijderKnop;
    public Toggle voltooidToggle; // Voeg dit toe aan prefab

    // Event dat wordt afgevuurd als voltooid aan/uit wordt gezet
    public event Action<bool> onVoltooidChanged;

    [HideInInspector]
    public Taak taak;

    public void Setup(Taak t, Action<Taak> verwijderCallback)
    {
        taak = t;

        if (tekstText != null) tekstText.text = t.tekst;
        if (deadlineText != null) deadlineText.text = t.deadline;

        if (verwijderKnop != null)
        {
            verwijderKnop.onClick.RemoveAllListeners();
            verwijderKnop.onClick.AddListener(() =>
            {
                verwijderCallback?.Invoke(taak);
            });

        // Toggle event instellen
        if (voltooidToggle != null)
        {
            voltooidToggle.onValueChanged.RemoveAllListeners();
            voltooidToggle.onValueChanged.AddListener((value) => onVoltooidChanged?.Invoke(value));
        }
        }
    }

    public void SetVoltooid(bool isVoltooid)
    {
        if (voltooidToggle != null)
            voltooidToggle.isOn = isVoltooid;

        tekstText.color = isVoltooid ? Color.gray : Color.black;
    }
}
