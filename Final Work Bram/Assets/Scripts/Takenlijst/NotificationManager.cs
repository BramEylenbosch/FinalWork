using UnityEngine;
using Unity.Notifications.Android;

public class NotificationManager : MonoBehaviour
{
    private void Start()
    {
        // Channel registreren bij opstart
        var channel = new AndroidNotificationChannel()
        {
            Id = "taak_channel",
            Name = "Taak Notificaties",
            Importance = Importance.High,
            Description = "Herinneringen voor taken"
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public void MaakNotificatie(string titel, string tekst, System.DateTime tijd)
    {
        var notification = new AndroidNotification()
        {
            Title = titel,
            Text = tekst,
            FireTime = tijd
        };

        AndroidNotificationCenter.SendNotification(notification, "taak_channel");
    }
}
