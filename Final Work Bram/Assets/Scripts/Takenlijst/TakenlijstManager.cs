using System;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject taskPanel;

    [Header("Task Panel")]
public Button sluitTaskPanelKnop;
public TextMeshProUGUI datumTitelText;



    [Header("Panel en knoppen")]
    public GameObject taakToevoegPanel;
    public Button openTaakPanelKnop;
    public Toggle herhaalDagelijksToggle;
    public Button bevestigToevoegenKnop;
    public Button annuleerToevoegenKnop;

    [Header("Calendar")]
    public GameObject calendarPanel;

    [Header("Datum kiezen")]
    public Button kiesDatumKnop;
    public TextMeshProUGUI gekozenDatumText;

    [Header("Notificaties")]
    public NotificationManager notificationManager;

    [Header("Rol")]
    public bool isMantelzorger = false;

private List<TaakItemController> taakItems = new();

// 🔐 Blijft ALTIJD intact
public List<Taak> alleTaken = new();

// 👁️ Wordt gefilterd
private List<Taak> zichtbareTaken = new();


    private FirestoreTakenService firestoreService;

    private IDatePicker _datePicker;
    private string geselecteerdeDatum = "";

    private float yStart = 95f;
    private float ySpacing = 195f;

    private void Start()
    {
        firestoreService = new FirestoreTakenService();

        // ❗ eerst alles loskoppelen
        openTaakPanelKnop.onClick.RemoveAllListeners();
        bevestigToevoegenKnop.onClick.RemoveAllListeners();
        annuleerToevoegenKnop.onClick.RemoveAllListeners();
        kiesDatumKnop.onClick.RemoveAllListeners();
        sluitTaskPanelKnop.onClick.RemoveAllListeners();

        // ❗ daarna pas koppelen
        openTaakPanelKnop.onClick.AddListener(OpenToevoegPanel);
        bevestigToevoegenKnop.onClick.AddListener(BevestigTaakToevoegen);
        annuleerToevoegenKnop.onClick.AddListener(SluitToevoegPanel);
        kiesDatumKnop.onClick.AddListener(OpenDatePicker);
        sluitTaskPanelKnop.onClick.AddListener(SluitTaskPanel);

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
    public void ToonTaskPanel(string datum)
{
    datumTitelText.text = datum;
    taskPanel.SetActive(true);
}

public void SluitTaskPanel()
{
    taskPanel.SetActive(false);
}

private bool BestaatTaakVandaag(Taak basisTaak)
{
    string vandaag = DateTime.Today.ToString("dd-MM-yyyy");
    return alleTaken.Any(t =>
        t.tekst == basisTaak.tekst &&
        t.deadline == vandaag
    );
}



    public void OpenToevoegPanel()
    {
        if (!isMantelzorger) return;

        taakToevoegPanel.SetActive(true);

        if (calendarPanel != null)
            calendarPanel.SetActive(false);

        if (taskPanel != null)
            taskPanel.SetActive(false);

        openTaakPanelKnop.gameObject.SetActive(false);

        taakInputField.text = "";
        geselecteerdeDatum = "";
        gekozenDatumText.text = "Geen datum gekozen";
    }


    public void SluitToevoegPanel()
    {
        taakToevoegPanel.SetActive(false);

        if (calendarPanel != null)
            calendarPanel.SetActive(true);

        openTaakPanelKnop.gameObject.SetActive(true);
    }


private async void BevestigTaakToevoegen()
{
    string tekst = taakInputField.text.Trim();
    if (string.IsNullOrEmpty(tekst)) return;

    string datum = geselecteerdeDatum; // gekozen datum
    if (string.IsNullOrEmpty(datum))
        datum = DateTime.Today.ToString("dd-MM-yyyy");

    // Check op duplicaten
    if (alleTaken.Any(t => t.tekst == tekst && t.deadline == datum))
    {
        Debug.Log("Taak bestaat al voor deze datum, toevoegen overgeslagen.");
        SluitToevoegPanel();
        return;
    }

    Taak nieuweTaak = new Taak
    {
        id = Guid.NewGuid().ToString(),
        tekst = tekst,
        deadline = datum,
        voltooid = false,
        herhaalDagelijks = herhaalDagelijksToggle.isOn,
    };

    await firestoreService.VoegTaakToe(nieuweTaak);

    alleTaken.Add(nieuweTaak); // meteen toevoegen aan lokale lijst
    if (datum == DateTime.Today.ToString("dd-MM-yyyy"))
        zichtbareTaken.Add(nieuweTaak);

    PlanNotificatiesVoorVandaag(nieuweTaak);
    SluitToevoegPanel();
    RenderTaken(zichtbareTaken);
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

private void RenderTaken(List<Taak> taken)
{
    foreach (var item in taakItems)
        Destroy(item.gameObject);
    taakItems.Clear();

    foreach (var taak in taken)
        MaakTaakItem(taak);
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

        Taak taak = zichtbareTaken[index]; // ✅ JUIST

        notificationManager?.AnnuleerNotificatiesVoorTaak(taak.tekst);

        await firestoreService.VerwijderTaak(taak.id);
        HerlaadTaken();
    }

private async void HerlaadTaken()
{
    // Clear UI
    foreach (var item in taakItems)
        Destroy(item.gameObject);
    taakItems.Clear();
    zichtbareTaken.Clear();

    // Laad alle taken uit Firestore
    alleTaken.Clear();
    var taken = await firestoreService.LaadTaken();
    alleTaken.AddRange(taken);

    List<Taak> takenOmTeRenderen = new List<Taak>();

    foreach (var taak in alleTaken)
    {
        // Dagelijkse herhaling check
        if (taak.herhaalDagelijks &&
            DateTime.TryParseExact(taak.deadline, "dd-MM-yyyy", null,
                System.Globalization.DateTimeStyles.None, out DateTime taakDatum))
        {
            if (taakDatum < DateTime.Today && !BestaatTaakVandaag(taak))
            {
                Taak nieuweDagTaak = new Taak
                {
                    id = Guid.NewGuid().ToString(),
                    tekst = taak.tekst,
                    deadline = DateTime.Today.ToString("dd-MM-yyyy"),
                    voltooid = false,
                    herhaalDagelijks = true
                };

                await firestoreService.VoegTaakToe(nieuweDagTaak);
                alleTaken.Add(nieuweDagTaak);
                takenOmTeRenderen.Add(nieuweDagTaak);
                continue; // overslaan van oude taak
            }
        }

        takenOmTeRenderen.Add(taak);
    }

    // Render alle taken die relevant zijn voor vandaag / gekozen datum
    RenderTaken(takenOmTeRenderen);
}


public void ToonTakenVoorDag(List<Taak> takenVoorDag)
{
    RenderTaken(takenVoorDag);
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

public void ToonTaskPanel()
{
    if (taskPanel != null)
        taskPanel.SetActive(true);
}

public void VerbergTaskPanel()
{
    if (taskPanel != null)
        taskPanel.SetActive(false);
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
