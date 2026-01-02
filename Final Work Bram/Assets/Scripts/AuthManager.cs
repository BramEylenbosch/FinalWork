using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro; // Voor TextMeshPro
using UnityEngine.UI; // Voor Button
using UnityEngine.SceneManagement;
using Firebase.Extensions;
public class AuthManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI feedbackText;

    public void Register()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        FirebaseInit.auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCompleted && !task.IsFaulted)
            {
                feedbackText.text = "Gebruiker geregistreerd!";
                Debug.Log("Registration successful!");
            }
            else
            {
                feedbackText.text = "Registratie mislukt: " + task.Exception?.InnerException?.Message;
                Debug.LogError(task.Exception);
            }
        });
    }

public void Login()
{
    string email = emailInput.text;
    string password = passwordInput.text;

    FirebaseInit.auth.SignInWithEmailAndPasswordAsync(email, password)
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                feedbackText.text = "Login succesvol!";

                KoppelingService koppelingService = new KoppelingService();
                koppelingService
                    .KoppelMantelzorgerAanGebruiker(AppBootstrap.GebruikerId)
                    .ContinueWithOnMainThread(_ =>
                    {
                        SceneManager.LoadScene("MantelzorgerStartScene");
                    });
            }
            else
            {
                feedbackText.text = "Login mislukt";
            }
        });

    }
}
