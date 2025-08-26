using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public void LoginAsOudere()
    {
        SceneManager.LoadScene("Home");   // laad de Home scene
    }

    public void LoginAsMantelzorger()
    {
        SceneManager.LoadScene("MantelHome");  // laad de Mantelzorger scene
    }
}
