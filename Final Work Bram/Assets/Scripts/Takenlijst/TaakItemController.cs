using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TaakItemController : MonoBehaviour
{
    public TextMeshProUGUI taakText;
    public TextMeshProUGUI deadlineText; 
    public Toggle voltooidToggle;
    public Button deleteButton;

    private Action<TaakItemController> onDelete;

    public event Action<bool> onVoltooidChanged;

    public void Setup(string tekst, string deadline, Action<TaakItemController> onDelete)
    {
        taakText.text = tekst;
        deadlineText.text = string.IsNullOrEmpty(deadline) ? "" : $"{deadline}";
        this.onDelete = onDelete;

        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(() => onDelete(this));

        voltooidToggle.onValueChanged.RemoveAllListeners();
        voltooidToggle.onValueChanged.AddListener(OnToggleChanged);

        // Init style
        OnToggleChanged(voltooidToggle.isOn);
    }

    private void OnToggleChanged(bool isAan)
    {
        taakText.fontStyle = isAan ? FontStyles.Strikethrough : FontStyles.Normal;
        taakText.color = isAan ? Color.gray : Color.black;

        onVoltooidChanged?.Invoke(isAan);
    }

    public void SetVoltooid(bool isVoltooid)
    {
        voltooidToggle.isOn = isVoltooid;
    }
}
