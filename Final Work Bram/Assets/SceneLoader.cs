using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string firstTimeScene = "MantelzorgerLogin"; 
    public string normalScene = "HomeScene"; 

    void Start()
    {

        if (!PlayerPrefs.HasKey("FirstTime"))
        {

            PlayerPrefs.SetInt("FirstTime", 1);
            PlayerPrefs.Save();

            SceneManager.LoadScene(firstTimeScene);
        }
        else
        {
            SceneManager.LoadScene(normalScene);
        }
    }
}
