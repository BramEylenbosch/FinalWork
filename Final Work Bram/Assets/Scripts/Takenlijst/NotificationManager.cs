using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class NotificationManager : MonoBehaviour
{
    private Dictionary<string, List<int>> taakNotificatieIds = new Dictionary<string, List<int>>();

    private void Start()
    {
#if UNITY_ANDROID
        var channel = new AndroidNotificationChannel()
        {
            Id = "taak_channel",
            Name = "Taak Notificaties",
            Importance = Importance.High,
            Description = "Herinneringen voor taken"
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
    }

    public void MaakNotificatie(string taakNaam, string titel, string tekst, DateTime tijd)
    {
#if UNITY_ANDROID
        var notification = new AndroidNotification()
        {
            Title = titel,
            Text = tekst,
            FireTime = tijd
        };

        int id = AndroidNotificationCenter.SendNotification(notification, "taak_channel");

        if (!taakNotificatieIds.ContainsKey(taakNaam))
            taakNotificatieIds[taakNaam] = new List<int>();

        taakNotificatieIds[taakNaam].Add(id);
#else
        Debug.Log($"[NOTIFICATIE - SIMULATIE] {titel} | {tekst} | {tijd}");
#endif
    }

    public void AnnuleerNotificatiesVoorTaak(string taakNaam)
    {
#if UNITY_ANDROID
        if (taakNotificatieIds.ContainsKey(taakNaam))
        {
            foreach (int id in taakNotificatieIds[taakNaam])
            {
                AndroidNotificationCenter.CancelNotification(id);
            }
            taakNotificatieIds.Remove(taakNaam);
        }
#else
        Debug.Log($"[NOTIFICATIE - GEANNULEERD] {taakNaam}");
#endif
    }
}
