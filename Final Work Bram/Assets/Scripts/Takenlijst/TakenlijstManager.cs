using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DatePicker;

public class TaaklijstManager : MonoBehaviour
{
    [Header("UI elementen")]
    public Transform taakContainer;
    public GameObject taakItemPrefab;
    public TMP_InputField taakInputField;

    [Header("Panel en knoppen")]
    public GameObject taakToevoegPanel;
    public Button openTaakPanelKnop;
    public Button bevestigToevoegenKnop;
    public Button annuleerToevoegenKnop;

    [Header("Datum kiezen")]
    public Button kiesDatumKnop;
    public TextMeshProUGUI gekozenDatumText;

    private List<TaakItemController> taakItems = new();
    private List<TaakData> takenLijst = new();

    private IDatePicker _datePicker;
    private string geselecteerdeDatum = "";

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
        // Knoppen listeners instellen
        openTaakPanelKnop.onClick.RemoveAllListeners();
        openTaakPanelKnop.onClick.AddListener(OpenToevoegPanel);

        bevestigToevoegenKnop.onClick.RemoveAllListeners();
        bevestigToevoegenKnop.onClick.AddListener(BevestigTaakToevoegen);

        annuleerToevoegenKnop.onClick.RemoveAllListeners();
        annuleerToevoegenKnop.onClick.AddListener(SluitToevoegPanel);

        // DatePicker instellen
#if UNITY_EDITOR
        _datePicker = new UnityEditorCalendar();
#elif UNITY_ANDROID
        _datePicker = new DatePicker.AndroidDatePicker();
#endif

        kiesDatumKnop.onClick.RemoveAllListeners();
        kiesDatumKnop.onClick.AddListener(OpenDatePicker);

        taakToevoegPanel.SetActive(false);
        LaadTaken();
    }

    private void OpenToevoegPanel()
    {
        taakToevoegPanel.SetActive(true);
        taakContainer.gameObject.SetActive(false);
        openTaakPanelKnop.gameObject.SetActive(false);

        // Reset invoervelden
        taakInputField.text = "";
        geselecteerdeDatum = "";
        gekozenDatumText.text = "Geen datum gekozen";
    }

    private void SluitToevoegPanel()
    {
        taakToevoegPanel.SetActive(false);
        taakInputField.text = "";
        geselecteerdeDatum = "";
        gekozenDatumText.text = "Geen datum gekozen";

        taakContainer.gameObject.SetActive(true);
        openTaakPanelKnop.gameObject.SetActive(true);
    }

    private void BevestigTaakToevoegen()
    {
        string nieuweTaak = taakInputField.text.Trim();

        if (string.IsNullOrEmpty(nieuweTaak)) return;

        TaakData nieuweTaakData = new TaakData
        {
            tekst = nieuweTaak,
            deadline = string.IsNullOrEmpty(geselecteerdeDatum) ? "" : geselecteerdeDatum,
            voltooid = false
        };

        takenLijst.Add(nieuweTaakData);
        MaakTaakItem(nieuweTaakData);

        SlaTakenOp();
        SluitToevoegPanel();
    }

    private void OpenDatePicker()
    {
        _datePicker?.Show(DateTime.Now, OnDateSelected);
    }

    private void OnDateSelected(DateTime value)
    {
        geselecteerdeDatum = value.ToString("dd-MM-yyyy");
        gekozenDatumText.text = geselecteerdeDatum;
        Debug.Log("Datum gekozen: " + geselecteerdeDatum);
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

#if UNITY_EDITOR
// Mock klasse om in de Editor te kunnen testen
class UnityEditorCalendarTaak : IDatePicker
{
    public void Show(DateTime initDate, Action<DateTime> callback)
    {
        callback?.Invoke(initDate);
    }
}
#endif
