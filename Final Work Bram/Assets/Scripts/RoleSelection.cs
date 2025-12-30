using UnityEngine;
using UnityEngine.SceneManagement;

public class RoleSelection : MonoBehaviour
{
    public void KiesGebruiker()
    {
        PlayerPrefs.SetString("UserRole", "Gebruiker");
        PlayerPrefs.Save();
        SceneManager.LoadScene("GebruikerStartScene");
        Debug.Log("UserRole gezet op: " + PlayerPrefs.GetString("UserRole"));
    }

public void KiesMantelzorger()
{
    PlayerPrefs.SetString("UserRole", "Mantelzorger");
    PlayerPrefs.Save();
    SceneManager.LoadScene("MantelzorgerStartScene");
}

}
