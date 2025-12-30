using UnityEngine;
using UnityEngine.SceneManagement;

public class AppStartManager : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("AppStartManager Awake");

        // Als er al een rol is gekozen → Home overslaan
        if (PlayerPrefs.HasKey("UserRole"))
        {
            string role = PlayerPrefs.GetString("UserRole");
            Debug.Log("Opgeslagen rol gevonden: " + role);

            if (role == "Gebruiker")
            {
                SceneManager.LoadScene("GebruikerStartScene");
            }
            else if (role == "Mantelzorger")
            {
                SceneManager.LoadScene("MantelzorgerStartScene");
            }
        }
        else
        {
            Debug.Log("Geen rol gevonden → Home blijft zichtbaar");
            // niets doen → HomeScene blijft
        }
    }
}
