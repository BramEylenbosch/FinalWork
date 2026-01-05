using UnityEngine;
using UnityEngine.SceneManagement;

public class RoleSelection : MonoBehaviour
{
    public void KiesGebruiker()
    {
        UserContext.UserRole = "Gebruiker";

        if (string.IsNullOrEmpty(UserContext.UserId))
            GebruikerInitializer.Instance.MaakNieuweGebruiker();

        SceneManager.LoadScene("GebruikerStartScene");
    }

    public void KiesMantelzorger()
    {
        UserContext.UserRole = "Mantelzorger";

        if (string.IsNullOrEmpty(UserContext.UserId))
            GebruikerInitializer.Instance.MaakNieuweGebruiker();

        SceneManager.LoadScene("MantelzorgerStartScene");
    }

}
