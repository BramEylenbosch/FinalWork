using System.Collections.Generic;
using UnityEngine;

public class TaaklijstManager : MonoBehaviour
{
    public Transform taakContainer;
    public GameObject taakItemPrefab;
    public TMPro.TMP_InputField taakInputField;

    private List<TaakItemController> taakItems = new();

    public void VoegTaakToe()
    {
        string nieuweTaak = taakInputField.text.Trim();
        if (string.IsNullOrEmpty(nieuweTaak)) return;

        GameObject taakGO = Instantiate(taakItemPrefab, taakContainer);
        TaakItemController taakItem = taakGO.GetComponent<TaakItemController>();
        taakItem.Setup(nieuweTaak, VerwijderTaak);
        taakItems.Add(taakItem);

        taakInputField.text = "";
    }

    private void VerwijderTaak(TaakItemController taakItem)
    {
        taakItems.Remove(taakItem);
        Destroy(taakItem.gameObject);
    }
}
