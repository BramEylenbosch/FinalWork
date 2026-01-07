using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DatePicker;
using Firebase.Firestore;
using System.Threading.Tasks;

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

    // Wordt true voor gebruikers die alleen read-only takenlijst zien
    public bool isReadOnly => !isMantelzorger;

    private List<TaakItemController> taakItems = new();

    // 🔐 Alle taken
    public List<Taak> alleTaken = new();

    // 👁️ Filtered voor UI
    private List<Taak> zichtbareTaken = new();

    private FirestoreTakenService firestoreService;
    private IDatePicker _datePicker;
    private string geselecteerdeDatum = "";

    private float yStart = 95f;
    private float ySpacing = 195f;

    private async void Start()
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

        // UI aanpassen voor read-only gebruiker
        if (isReadOnly)
        {
            openTaakPanelKnop.gameObject.SetActive(false);
            taakToevoegPanel.SetActive(false);
            herhaalDagelijksToggle.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        _datePicker = new UnityEditorCalendarTaak();
#elif UNITY_ANDROID
        _datePicker = new DatePicker.AndroidDatePicker();
#endif

        // Controleer of gebruiker gekoppeld is aan mantelzorger
        if (isReadOnly && string.IsNullOrEmpty(UserContext.CaretakerId))
        {
            Debug.LogWarning("[TaaklijstManager] CaretakerId is leeg, geen taken te laden!");
            return;
        }

        // Taken laden
        await HerlaadTaken();

        // Testnotificatie
        if (notificationManager != null && isMantelzorger)
        {
            InvokeRepeating(nameof(StuurTestNotificatie), 5f, 60f);
        }
    }

    private void StuurTestNotificatie()
    {
        notificationManager?.MaakNotificatie(
            "TestTaak",
            "Test herinnering",
            "Dit is een testnotificatie.",
            DateTime.Now.AddSeconds(2)
        );
    }

    public void OpenToevoegPanel()
    {
        if (isReadOnly) return;

        taakToevoegPanel.SetActive(true);
        calendarPanel?.SetActive(false);
        taskPanel?.SetActive(false);
        openTaakPanelKnop.gameObject.SetActive(false);

        taakInputField.text = "";
        geselecteerdeDatum = "";
        gekozenDatumText.text = "Geen datum gekozen";
    }

public void SluitTaskPanel()
{
    if (taskPanel != null)
        taskPanel.SetActive(false);

    ToonCalendarPanel(); // altijd terug naar kalender
}
 public void SluitToevoegPanel()
{
    if (taakToevoegPanel != null)
        taakToevoegPanel.SetActive(false);

    ToonCalendarPanel(); // altijd terug naar kalender
}
    public void VerbergCalendarPanel()
    {
        if (calendarPanel != null)
            calendarPanel.SetActive(false);
    }
        public void VerbergTaskPanel()
    {
        if (taskPanel != null)
            taskPanel.SetActive(false);
    }

    public void VerbergToevoegPanel()
    {
        if (taakToevoegPanel != null)
            taakToevoegPanel.SetActive(false);
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

    private async void BevestigTaakToevoegen()
    {
        if (isReadOnly) return;

        string tekst = taakInputField.text.Trim();
        if (string.IsNullOrEmpty(tekst)) return;

        string datum = string.IsNullOrEmpty(geselecteerdeDatum)
            ? DateTime.Today.ToString("dd-MM-yyyy")
            : geselecteerdeDatum;

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
        alleTaken.Add(nieuweTaak);
        LocalCache.SaveTaken(alleTaken);

        if (datum == DateTime.Today.ToString("dd-MM-yyyy"))
            zichtbareTaken.Add(nieuweTaak);

        PlanNotificatiesVoorVandaag(nieuweTaak);
        SluitToevoegPanel();
        RenderTaken(zichtbareTaken);
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
        if (taakItem != null)
        {
            Action<Taak> verwijderCallback = isMantelzorger ? VerwijderTaak : null;
            taakItem.Setup(taak, verwijderCallback);

            // Toggle interactable uitzetten voor gebruiker
            if (isReadOnly && taakItem.voltooidToggle != null)
                taakItem.voltooidToggle.interactable = false;

            taakItems.Add(taakItem);

            int index = taakItems.Count - 1;
            Vector3 pos = taakGO.transform.localPosition;
            pos.y = yStart - index * ySpacing;
            taakGO.transform.localPosition = pos;
        }
    }

    private async void VerwijderTaak(Taak taak)
    {
        if (taak == null) return;
        notificationManager?.AnnuleerNotificatiesVoorTaak(taak.tekst);
        await firestoreService.VerwijderTaak(taak.id);

        alleTaken.Remove(taak);
        zichtbareTaken.Remove(taak);

        TaakItemController item = taakItems.Find(i => i.taak == taak);
        if (item != null)
        {
            taakItems.Remove(item);
            Destroy(item.gameObject);
        }

        LocalCache.SaveTaken(alleTaken);
    }

    public async Task HerlaadTaken()
    {
        foreach (var item in taakItems)
            Destroy(item.gameObject);
        taakItems.Clear();
        zichtbareTaken.Clear();
        alleTaken.Clear();

        List<Taak> taken = new List<Taak>();

        try
        {
            taken = await firestoreService.LaadTaken();
            Debug.Log($"[TaaklijstManager] {taken.Count} taken van Firebase geladen.");
        }
        catch
        {
            Debug.LogWarning("[TaaklijstManager] Firebase laden mislukt, gebruik lokaal");
            taken = LocalCache.LoadTaken();
        }

        alleTaken.AddRange(taken);

        // Dagelijkse herhaling check
        List<Taak> takenOmTeRenderen = new List<Taak>();
        foreach (var taak in alleTaken)
        {
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

                    if (isMantelzorger)
                        await firestoreService.VoegTaakToe(nieuweDagTaak);

                    alleTaken.Add(nieuweDagTaak);
                    takenOmTeRenderen.Add(nieuweDagTaak);
                    continue;
                }
            }

            takenOmTeRenderen.Add(taak);
        }

        RenderTaken(takenOmTeRenderen);
        LocalCache.SaveTaken(alleTaken);
    }

    private void PlanNotificatiesVoorVandaag(Taak taak)
    {
        if (notificationManager == null || string.IsNullOrEmpty(taak.deadline)) return;

        if (!DateTime.TryParseExact(taak.deadline, "dd-MM-yyyy", null,
            System.Globalization.DateTimeStyles.None, out DateTime deadline)) return;

        if (deadline.Date != DateTime.Today) return;

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

    public void ToonTaskPanel(string datum = "")
    {
        taskPanel?.SetActive(true);
        if (!string.IsNullOrEmpty(datum))
            datumTitelText.text = datum;
    }

public void ToonCalendarPanel()
{
    if (calendarPanel != null)
        calendarPanel.SetActive(true);

    if (taskPanel != null)
        taskPanel.SetActive(false);

    if (taakToevoegPanel != null)
        taakToevoegPanel.SetActive(false);
}


    private bool BestaatTaakVandaag(Taak basisTaak)
    {
        string vandaag = DateTime.Today.ToString("dd-MM-yyyy");
        return alleTaken.Any(t => t.tekst == basisTaak.tekst && t.deadline == vandaag);
    }
    // Voeg dit onderaan je TaaklijstManager class toe
    public void ToonTakenVoorDag(List<Taak> takenVoorDag)
    {
        RenderTaken(takenVoorDag);
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
