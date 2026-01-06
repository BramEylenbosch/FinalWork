using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DatePicker;
using Firebase.Firestore;

public class TaaklijstManager : MonoBehaviour
{
    [Header("UI elementen")]
    public Transform taakContainer;
    public GameObject taakItemPrefab;
    public TMP_InputField taakInputField;

    [Header("Panel en knoppen")]
    public GameObject taakToevoegPanel;
    public Button openTaakPanelKnop;
    public Toggle herhaalDagelijksToggle;
    public Button bevestigToevoegenKnop;
    public Button annuleerToevoegenKnop;

    [Header("Datum kiezen")]
    public Button kiesDatumKnop;
    public TextMeshProUGUI gekozenDatumText;

    [Header("Notificaties")]
    public NotificationManager notificationManager;

    [Header("Rol")]
    public bool isMantelzorger = false;

    private List<TaakItemController> taakItems = new();
    private List<Taak> takenLijst = new();

    private FirestoreTakenService firestoreService;

    private IDatePicker _datePicker;
    private string geselecteerdeDatum = "";

    private float yStart = 95f;
    private float ySpacing = 195f;

    private void Start()
    {
        firestoreService = new FirestoreTakenService();

        // UI listeners
        openTaakPanelKnop.onClick.AddListener(OpenToevoegPanel);
        bevestigToevoegenKnop.onClick.AddListener(BevestigTaakToevoegen);
        annuleerToevoegenKnop.onClick.AddListener(SluitToevoegPanel);
        kiesDatumKnop.onClick.AddListener(OpenDatePicker);

        taakToevoegPanel.SetActive(false);

#if UNITY_EDITOR
        _datePicker = new UnityEditorCalendarTaak();
#elif UNITY_ANDROID
        _datePicker = new DatePicker.AndroidDatePicker();
#endif

        HerlaadTaken();

        // 🔔 Testnotificatie
        if (notificationManager != null)
        {
            InvokeRepeating(nameof(StuurTestNotificatie), 5f, 60f);
        }
    }

    private void StuurTestNotificatie()
    {
        notificationManager.MaakNotificatie(
            "TestTaak",
            "Test herinnering",
            "Dit is een testnotificatie.",
            DateTime.Now.AddSeconds(2)
        );
    }

    private void OpenToevoegPanel()
    {
        Debug.Log("Klik ontvangen, isMantelzorger = " + isMantelzorger);
        if (!isMantelzorger) return;

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

    private async void BevestigTaakToevoegen()
    {
        string tekst = taakInputField.text.Trim();
        if (string.IsNullOrEmpty(tekst)) return;

        Taak nieuweTaak = new Taak
        {
            id = Guid.NewGuid().ToString(),
            tekst = tekst,
            deadline = geselecteerdeDatum,
            voltooid = false,
            herhaalDagelijks = herhaalDagelijksToggle.isOn,
        };


        await firestoreService.VoegTaakToe(nieuweTaak);

        PlanNotificatiesVoorVandaag(nieuweTaak);
        SluitToevoegPanel();
        HerlaadTaken();
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

    private void MaakTaakItem(Taak taak)
    {
        GameObject taakGO = Instantiate(taakItemPrefab, taakContainer);
        TaakItemController taakItem = taakGO.GetComponent<TaakItemController>();

        if (isMantelzorger)
            taakItem.Setup(taak.tekst, taak.deadline, VerwijderTaak);
        else
            taakItem.Setup(taak.tekst, taak.deadline, null);

        taakItem.SetVoltooid(taak.voltooid);

        taakItem.onVoltooidChanged += async (isVoltooid) =>
        {
            taak.voltooid = isVoltooid;
            await firestoreService.VoegTaakToe(taak);
        };

        taakItems.Add(taakItem);

        int index = taakItems.Count - 1;
        Vector3 pos = taakGO.transform.localPosition;
        pos.y = yStart - index * ySpacing;
        taakGO.transform.localPosition = pos;
    }

    private async void VerwijderTaak(TaakItemController taakItem)
    {
        int index = taakItems.IndexOf(taakItem);
        if (index < 0) return;

        Taak taak = takenLijst[index];

        notificationManager?.AnnuleerNotificatiesVoorTaak(taak.tekst);

        await firestoreService.VerwijderTaak(taak.id);
        HerlaadTaken();
    }

private async void HerlaadTaken()
{
    foreach (var item in taakItems)
        Destroy(item.gameObject);

    taakItems.Clear();
    takenLijst.Clear();

    var taken = await firestoreService.LaadTaken();

    for (int i = 0; i < taken.Count; i++)
    {
        var taak = taken[i];

        // ✅ Dagelijkse herhaling check
        if (taak.herhaalDagelijks && !string.IsNullOrEmpty(taak.deadline))
        {
            if (DateTime.TryParseExact(
                    taak.deadline, 
                    "dd-MM-yyyy", 
                    null, 
                    System.Globalization.DateTimeStyles.None, 
                    out DateTime taakDatum))
            {
                if (taakDatum < DateTime.Today)
                {
                    // Maak nieuwe taak voor vandaag
                    Taak nieuweDagTaak = new Taak
                    {
                        id = Guid.NewGuid().ToString(),
                        tekst = taak.tekst,
                        deadline = DateTime.Today.ToString("dd-MM-yyyy"),
                        voltooid = false,
                        herhaalDagelijks = true
                    };

                    await firestoreService.VoegTaakToe(nieuweDagTaak);

                    // Vervang oude taak voor UI
                    taak = nieuweDagTaak;
                    taken[i] = nieuweDagTaak;
                }
            }
        }

        takenLijst.Add(taak);
        MaakTaakItem(taak);
    }
}



    private void PlanNotificatiesVoorVandaag(Taak taak)
    {
        if (notificationManager == null || string.IsNullOrEmpty(taak.deadline)) return;

        if (DateTime.TryParseExact(
            taak.deadline,
            "dd-MM-yyyy",
            null,
            System.Globalization.DateTimeStyles.None,
            out DateTime deadline))
        {
            if (deadline.Date != DateTime.Now.Date) return;

            DateTime volgende = DateTime.Now.AddMinutes(10);

            while (volgende.Date == DateTime.Now.Date)
            {
                notificationManager.MaakNotificatie(
                    taak.tekst,
                    "Herinnering",
                    $"Vergeet '{taak.tekst}' niet te voltooien!",
                    volgende
                );
                volgende = volgende.AddMinutes(10);
            }
        }
    }
}

#if UNITY_EDITOR
class UnityEditorCalendarTaak : IDatePicker
{
    public void Show(DateTime initDate, Action<DateTime> callback)
    {
        callback?.Invoke(initDate);
    }
}
#endif
