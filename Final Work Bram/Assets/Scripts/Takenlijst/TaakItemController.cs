using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TaakItemController : MonoBehaviour
{
    public TextMeshProUGUI tekstText;
    public TextMeshProUGUI deadlineText;
    public Button verwijderKnop;
    public Toggle voltooidToggle; 


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
        verwijderKnop.gameObject.SetActive(verwijderCallback != null);

        verwijderKnop.onClick.RemoveAllListeners();
        if (verwijderCallback != null)
        {
            verwijderKnop.onClick.AddListener(() =>
            {
                verwijderCallback?.Invoke(taak);
            });
        }
    }

    if (voltooidToggle != null)
    {
        voltooidToggle.onValueChanged.RemoveAllListeners();
        voltooidToggle.onValueChanged.AddListener((value) => onVoltooidChanged?.Invoke(value));
    }
}

    public void SetVoltooid(bool isVoltooid)
    {
        if (voltooidToggle != null)
            voltooidToggle.isOn = isVoltooid;

        tekstText.color = isVoltooid ? Color.gray : Color.black;
    }
}
