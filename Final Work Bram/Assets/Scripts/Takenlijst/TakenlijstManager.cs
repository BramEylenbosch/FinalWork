using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaaklijstManager : MonoBehaviour
{
    [Header("UI elementen")]
    public Transform taakContainer;
    public GameObject taakItemPrefab;
    public TMP_InputField taakInputField;
    public TMP_InputField deadlineInputField;

    private List<TaakItemController> taakItems = new();
    private List<TaakData> takenLijst = new();

    [System.Serializable]
    public class TaakData
    {
        public string tekst;
        public string deadline;
        public bool voltooid;
    }

    [System.Serializable]
    private class TaakDataWrapper
    {
        public List<TaakData> taken;
    }

    private void Start()
    {
        LaadTaken();
    }

    public void VoegTaakToe()
    {
        string nieuweTaak = taakInputField.text.Trim();
        string deadline = deadlineInputField.text.Trim();

        if (string.IsNullOrEmpty(nieuweTaak)) return;

        TaakData nieuweTaakData = new TaakData
        {
            tekst = nieuweTaak,
            deadline = deadline,
            voltooid = false
        };

        takenLijst.Add(nieuweTaakData);
        MaakTaakItem(nieuweTaakData);

        taakInputField.text = "";
        deadlineInputField.text = "";

        SlaTakenOp();
    }

    private void MaakTaakItem(TaakData taakData)
    {
        GameObject taakGO = Instantiate(taakItemPrefab, taakContainer);
        TaakItemController taakItem = taakGO.GetComponent<TaakItemController>();
        taakItem.Setup(taakData.tekst, taakData.deadline, VerwijderTaak);
        taakItem.SetVoltooid(taakData.voltooid);
        taakItem.onVoltooidChanged += (isVoltooid) =>
        {
            taakData.voltooid = isVoltooid;
            SlaTakenOp();
        };
        taakItems.Add(taakItem);
    }

    private void VerwijderTaak(TaakItemController taakItem)
    {
        int index = taakItems.IndexOf(taakItem);
        if (index >= 0)
        {
            taakItems.RemoveAt(index);
            takenLijst.RemoveAt(index);
            Destroy(taakItem.gameObject);
            SlaTakenOp();
        }
    }

    public void SlaTakenOp()
    {
        TaakDataWrapper wrapper = new TaakDataWrapper { taken = takenLijst };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString("takenlijst", json);
        PlayerPrefs.Save();
    }

    public void LaadTaken()
    {
        // Eerst alle oude taakitems verwijderen uit de UI
        foreach (var taakItem in taakItems)
        {
            Destroy(taakItem.gameObject);
        }
        taakItems.Clear();
        takenLijst.Clear();

        if (PlayerPrefs.HasKey("takenlijst"))
        {
            string json = PlayerPrefs.GetString("takenlijst");
            TaakDataWrapper wrapper = JsonUtility.FromJson<TaakDataWrapper>(json);

            if (wrapper.taken != null)
            {
                foreach (TaakData taak in wrapper.taken)
                {
                    takenLijst.Add(taak);
                    MaakTaakItem(taak);
                }
            }
        }
    }


}
