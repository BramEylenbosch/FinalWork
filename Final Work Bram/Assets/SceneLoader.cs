using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string firstTimeScene = "MantelzorgerLogin"; // de scene die je 1x wil tonen
    public string normalScene = "HomeScene"; // de scene voor daarna

    void Start()
    {
        // Check of de app voor het eerst is gestart
        if (!PlayerPrefs.HasKey("FirstTime"))
        {
            // Zet de key zodat we weten dat het de eerste keer is geweest
            PlayerPrefs.SetInt("FirstTime", 1);
            PlayerPrefs.Save();

            // Laad de eerste keer scene
            SceneManager.LoadScene(firstTimeScene);
        }
        else
        {
            // Laad de normale scene
            SceneManager.LoadScene(normalScene);
        }
    }
}
