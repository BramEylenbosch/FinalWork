using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalendarManager : MonoBehaviour
{
    public Transform calendarGridParent; // Assign CalendarPanel
    public GameObject dayButtonPrefab;
    public TaaklijstManager taaklijstManager; // Reference to your task manager
public GameObject taskPanel; // sleep hier het panel in de Inspector

    void Start()
    {
        CreateCalendar();
    }
    public void CreateCalendar()
    {
        for (int day = 1; day <= 31; day++)
        {
            GameObject dayGO = Instantiate(dayButtonPrefab, calendarGridParent);
            TMP_Text dayText = dayGO.GetComponentInChildren<TMP_Text>();
            dayText.text = day.ToString();

            int capturedDay = day;
            dayGO.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnDayClicked(capturedDay);
            });
        }
    }

private void OnDayClicked(int day)
{
    string dagString = day.ToString("D2") + "-" + DateTime.Now.ToString("MM-yyyy");

    var takenVoorDezeDag = taaklijstManager.takenLijst
        .Where(t => t.deadline == dagString)
        .ToList();

    taaklijstManager.ToonTakenVoorDag(takenVoorDezeDag);

    // Open het panel
    taaklijstManager.ToonTaskPanel();
}


}
