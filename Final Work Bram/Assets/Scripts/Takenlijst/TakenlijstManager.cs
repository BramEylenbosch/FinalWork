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

    [Header("Notificaties")]
    public NotificationManager notificationManager;

    [Header("Rol")]
    public bool isMantelzorger = false;

    private List<TaakItemController> taakItems = new List<TaakItemController>();
    private List<TaakData> takenLijst = new List<TaakData>();

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
        // UI listeners
        openTaakPanelKnop.onClick.AddListener(OpenToevoegPanel);
        bevestigToevoegenKnop.onClick.AddListener(BevestigTaakToevoegen);
        annuleerToevoegenKnop.onClick.AddListener(SluitToevoegPanel);
        kiesDatumKnop.onClick.AddListener(OpenDatePicker);

        taakToevoegPanel.SetActive(false);
        LaadTaken();

#if UNITY_EDITOR
        _datePicker = new UnityEditorCalendarTaak();
#elif UNITY_ANDROID
        _datePicker = new DatePicker.AndroidDatePicker();
#endif

        // 🔔 Testnotificatie om de minuut
        if (notificationManager != null)
        {
            InvokeRepeating(nameof(StuurTestNotificatie), 5f, 60f);
        }
    }

    private void StuurTestNotificatie()
    {
        notificationManager.MaakNotificatie(
            "Test herinnering",
            "Dit is een testnotificatie om te kijken of het werkt.",
            DateTime.Now.AddSeconds(2)
        );
    }

    private void OpenToevoegPanel()
    {
        taakToevoegPanel.SetActive(true);
        taakContainer.gameObject.SetActive(false);
        openTaakPanelKnop.gameObject.SetActive(false);

        taakInputField.text = "";
        geselecteerdeDatum = "";
        gekozenDatumText.text = "Geen datum gekozen";
    }

    private void SluitToevoegPanel()
    {
        taakToevoegPanel.SetActive(false);
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
            deadline = geselecteerdeDatum,
            voltooid = false
        };

        takenLijst.Add(nieuweTaakData);
        MaakTaakItem(nieuweTaakData);
        PlanNotificatiesVoorVandaag(nieuweTaakData);

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
    }

    private void MaakTaakItem(TaakData taakData)
    {
        GameObject taakGO = Instantiate(taakItemPrefab, taakContainer);
        TaakItemController taakItem = taakGO.GetComponent<TaakItemController>();

        // Alleen mantelzorger kan verwijderen
        if (isMantelzorger)
            taakItem.Setup(taakData.tekst, taakData.deadline, VerwijderTaak);
        else
            taakItem.Setup(taakData.tekst, taakData.deadline, null);

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
        taakItems.ForEach(item => Destroy(item.gameObject));
        taakItems.Clear();
        takenLijst.Clear();

        if (PlayerPrefs.HasKey("takenlijst"))
        {
            string json = PlayerPrefs.GetString("takenlijst");
            TaakDataWrapper wrapper = JsonUtility.FromJson<TaakDataWrapper>(json);
            if (wrapper?.taken != null)
            {
                foreach (var taak in wrapper.taken)
                {
                    takenLijst.Add(taak);
                    MaakTaakItem(taak);
                }
            }
        }
    }

    private void PlanNotificatiesVoorVandaag(TaakData taak)
    {
        if (notificationManager == null || string.IsNullOrEmpty(taak.deadline)) return;

        if (DateTime.TryParseExact(taak.deadline, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime deadline))
        {
            DateTime nu = DateTime.Now;
            if (deadline.Date == nu.Date)
            {
                // Plan notificaties elk minuut tot middernacht (voor testen)
                DateTime volgende = nu.AddMinutes(1);
                while (volgende.Date == nu.Date)
                {
                    notificationManager.MaakNotificatie(
                        "Herinnering",
                        $"Vergeet '{taak.tekst}' niet te voltooien!",
                        volgende
                    );
                    volgende = volgende.AddMinutes(1);
                }
            }
        }
    }
}

#if UNITY_EDITOR
class UnityEditorCalendarTaak : IDatePicker
{
    public void Show(DateTime initDate, Action<DateTime> callback) => callback?.Invoke(initDate);
}
#endif
