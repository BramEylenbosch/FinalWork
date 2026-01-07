using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AuthManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI feedbackText;

    public async void Login()
    {
        if (FirebaseInit.auth == null)
        {
            feedbackText.text = "Firebase nog niet klaar!";
            Debug.LogError("FirebaseInit.auth is null!");
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Vul email en wachtwoord in!";
            return;
        }

        try
        {
            var result = await FirebaseInit.auth.SignInWithEmailAndPasswordAsync(email, password);
            feedbackText.text = "Login succesvol: " + result.User.Email;
            Debug.Log("Login success: " + result.User.Email);

            if (UserContext.UserRole == "Mantelzorger")
                SceneManager.LoadScene("MantelzorgerStartScene");
            else
                SceneManager.LoadScene("GebruikerStartScene");
        }
        catch (Firebase.FirebaseException ex)
        {
            feedbackText.text = "Login mislukt: " + ex.Message;
            Debug.LogError("Firebase login error: " + ex);
        }
        catch (System.Exception ex)
        {
            feedbackText.text = "Login mislukt: " + ex.Message;
            Debug.LogError("Algemene login error: " + ex);
        }
    }

    public async void Register()
    {
        if (FirebaseInit.auth == null)
        {
            feedbackText.text = "Firebase nog niet klaar!";
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Vul email en wachtwoord in!";
            return;
        }

        try
        {
            var result = await FirebaseInit.auth.CreateUserWithEmailAndPasswordAsync(email, password);
            feedbackText.text = "Registratie succesvol: " + result.User.Email;
            Debug.Log("Registration success: " + result.User.Email);
        }
        catch (Firebase.FirebaseException ex)
        {
            feedbackText.text = "Registratie mislukt: " + ex.Message;
            Debug.LogError("Firebase registration error: " + ex);
        }
        catch (System.Exception ex)
        {
            feedbackText.text = "Registratie mislukt: " + ex.Message;
            Debug.LogError("Algemene registratie error: " + ex);
        }
    }
}
