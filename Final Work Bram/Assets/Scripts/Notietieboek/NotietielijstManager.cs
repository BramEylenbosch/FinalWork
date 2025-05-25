using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotitieLijstManager : MonoBehaviour
{
    public Transform notitieContainer; // De Content uit je ScrollView
    public GameObject notitiePrefab;   // Jouw prefab
    public TMP_InputField notitieInputField;

    public void VoegNotitieToe()
    {
        string tekst = notitieInputField.text;
        if (string.IsNullOrWhiteSpace(tekst)) return;

        GameObject nieuweNotitie = Instantiate(notitiePrefab, notitieContainer);
        nieuweNotitie.GetComponentInChildren<TMP_Text>().text = tekst;
        notitieInputField.text = "";
    }
}
