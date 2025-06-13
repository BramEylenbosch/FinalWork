using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaaklijstManager : MonoBehaviour
{
    public Transform taakContainer;
    public GameObject taakItemPrefab;
    public TMP_InputField taakInputField;
    public TMP_InputField deadlineInputField;

    private List<TaakItemController> taakItems = new();

    public void VoegTaakToe()
    {
        string nieuweTaak = taakInputField.text.Trim();
        string deadline = deadlineInputField.text.Trim();

        if (string.IsNullOrEmpty(nieuweTaak)) return;

        GameObject taakGO = Instantiate(taakItemPrefab, taakContainer);
        TaakItemController taakItem = taakGO.GetComponent<TaakItemController>();
        taakItem.Setup(nieuweTaak, deadline, VerwijderTaak);
        taakItems.Add(taakItem);

        taakInputField.text = "";
        deadlineInputField.text = "";
    }

    private void VerwijderTaak(TaakItemController taakItem)
    {
        taakItems.Remove(taakItem);
        Destroy(taakItem.gameObject);
    }
}
