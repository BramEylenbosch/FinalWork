using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaakItemController : MonoBehaviour
{
    public TextMeshProUGUI taakText;
    public Button deleteButton;

    public void Setup(string tekst, System.Action<TaakItemController> onDelete)
    {
        taakText.text = tekst;
        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(() => onDelete(this));
    }
}
