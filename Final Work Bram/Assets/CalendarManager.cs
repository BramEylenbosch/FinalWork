using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalendarManager : MonoBehaviour
{
    public Transform calendarGridParent;
    public GameObject dayButtonPrefab;
    public TaaklijstManager taaklijstManager; 
public GameObject taskPanel; 

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
    if (taaklijstManager != null)
        taaklijstManager.VerbergCalendarPanel();

    string dagString = day.ToString("D2") + "-" + DateTime.Now.ToString("MM-yyyy");


    var takenVoorDezeDag = taaklijstManager.alleTaken
        .Where(t => t.deadline.StartsWith(dagString))
        .ToList();

    taaklijstManager.ToonTaskPanel(dagString);
    taaklijstManager.ToonTakenVoorDag(takenVoorDezeDag);
}



}
