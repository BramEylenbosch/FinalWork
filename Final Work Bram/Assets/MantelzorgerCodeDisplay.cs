using UnityEngine;
using TMPro;

public class MantelzorgerCodeDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI codeText;

    private void Start()
    {
        ToonCode();
    }

    private void ToonCode()
    {
        if (UserContext.UserRole != "Mantelzorger")
        {
            codeText.text = "Alleen mantelzorgers hebben een code!";
            Debug.LogWarning("[MantelzorgerCodeDisplay] Niet ingelogd als mantelzorger.");
            return;
        }

        if (string.IsNullOrEmpty(UserContext.UserId))
        {
            codeText.text = "Mantelzorger-ID niet gevonden!";
            Debug.LogError("[MantelzorgerCodeDisplay] UserContext.UserId is leeg!");
            return;
        }

        // Toon de Firestore UserId van de mantelzorger
        string mantelzorgerCode = UserContext.UserId;
        codeText.text = "Deel deze code met de gebruiker:\n" + mantelzorgerCode;

        Debug.Log("[MantelzorgerCodeDisplay] Mantelzorger code: " + mantelzorgerCode);
    }
}
