using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaaklijstManager : MonoBehaviour
{
    [Header("UI elementen")]
    public Transform taakContainer;
    public GameObject taakItemPrefab;
    public TMP_InputField taakInputField;
    public TMP_InputField deadlineInputField;

    [Header("Panel en knoppen")]
    public GameObject taakToevoegPanel;
    public Button openTaakPanelKnop;
    public Button bevestigToevoegenKnop;
    public Button annuleerToevoegenKnop;

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

    private float yStart = 95f;
    private float ySpacing = 195f;

    private void Start()
    {
        openTaakPanelKnop.onClick.RemoveAllListeners();
        openTaakPanelKnop.onClick.AddListener(OpenToevoegPanel);

        bevestigToevoegenKnop.onClick.RemoveAllListeners();
        bevestigToevoegenKnop.onClick.AddListener(BevestigTaakToevoegen);

        annuleerToevoegenKnop.onClick.RemoveAllListeners();
        annuleerToevoegenKnop.onClick.AddListener(SluitToevoegPanel);

        taakToevoegPanel.SetActive(false);
        LaadTaken();
    }


    private void OpenToevoegPanel()
    {
        taakToevoegPanel.SetActive(true);
        taakContainer.gameObject.SetActive(false);
        openTaakPanelKnop.gameObject.SetActive(false);
    }

    private void SluitToevoegPanel()
    {
        taakToevoegPanel.SetActive(false);
        taakInputField.text = "";
        deadlineInputField.text = "";
        taakContainer.gameObject.SetActive(true);
        openTaakPanelKnop.gameObject.SetActive(true);
    }

    private void BevestigTaakToevoegen()
    {
        VoegTaakToe();
        SluitToevoegPanel();
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

        int index = taakItems.Count - 1;
        Vector3 pos = taakGO.transform.localPosition;
        pos.y = yStart - index * ySpacing;
        taakGO.transform.localPosition = pos;
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
