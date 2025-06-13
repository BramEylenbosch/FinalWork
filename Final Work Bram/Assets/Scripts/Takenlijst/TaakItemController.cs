using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaakItemController : MonoBehaviour
{
    public TextMeshProUGUI taakText;
    public TextMeshProUGUI deadlineText; // Alleen tonen, geen inputfield!
    public Toggle voltooidToggle;
    public Button deleteButton;

    private System.Action<TaakItemController> onDelete;

    public void Setup(string tekst, string deadline, System.Action<TaakItemController> onDelete)
    {
        taakText.text = tekst;
        deadlineText.text = string.IsNullOrEmpty(deadline) ? "" : $"Deadline: {deadline}";
        this.onDelete = onDelete;

        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(() => onDelete(this));

        voltooidToggle.onValueChanged.RemoveAllListeners();
        voltooidToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isAan)
    {
        taakText.fontStyle = isAan ? FontStyles.Strikethrough : FontStyles.Normal;
        taakText.color = isAan ? Color.gray : Color.black;
    }
}
