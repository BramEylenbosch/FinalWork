using UnityEngine;
using Unity.Notifications.Android;
using System.Collections.Generic;

public class NotificationManager : MonoBehaviour
{
    private Dictionary<string, List<int>> taakNotificatieIds = new Dictionary<string, List<int>>();

    private void Start()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "taak_channel",
            Name = "Taak Notificaties",
            Importance = Importance.High,
            Description = "Herinneringen voor taken"
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public void MaakNotificatie(string taakNaam, string titel, string tekst, System.DateTime tijd)
    {
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
    }

    public void AnnuleerNotificatiesVoorTaak(string taakNaam)
    {
        if (taakNotificatieIds.ContainsKey(taakNaam))
        {
            foreach (int id in taakNotificatieIds[taakNaam])
            {
                AndroidNotificationCenter.CancelNotification(id);
            }
            taakNotificatieIds.Remove(taakNaam);
        }
    }
}
