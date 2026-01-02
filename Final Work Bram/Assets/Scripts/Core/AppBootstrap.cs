using UnityEngine;
using System;

public class AppBootstrap : MonoBehaviour
{
    public static string GebruikerId { get; private set; }

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("gebruikerId"))
        {
            string nieuwId = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("gebruikerId", nieuwId);
            PlayerPrefs.Save();
        }

        GebruikerId = PlayerPrefs.GetString("gebruikerId");
        DontDestroyOnLoad(gameObject);
    }
}
