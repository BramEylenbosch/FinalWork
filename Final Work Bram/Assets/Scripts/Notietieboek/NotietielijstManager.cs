using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NotitieLijstManager : MonoBehaviour
{
    public Transform notitieContainer;         // ScrollView > Content
    public GameObject notitiePrefab;           // Prefab met InputField, Verwijderknop, Fotoknop & RawImage
    public TMP_InputField notitieInputField;   // Input bovenaan voor nieuwe notitie

    public void VoegNotitieToe()
    {
        string tekst = notitieInputField.text;
        if (string.IsNullOrWhiteSpace(tekst)) return;

        // Maak nieuwe notitie aan in lijst
        GameObject nieuweNotitie = Instantiate(notitiePrefab, notitieContainer);

        // Bewerkbare tekstveld instellen
        TMP_InputField inputField = nieuweNotitie.GetComponentInChildren<TMP_InputField>();
        inputField.text = tekst;

        // Verwijderknop koppelen
        Button deleteButton = nieuweNotitie.transform.Find("DeleteButton").GetComponent<Button>();
        deleteButton.onClick.AddListener(() => VerwijderNotitie(nieuweNotitie));

        // Fotoknop koppelen (optioneel)
        RawImage fotoImage = nieuweNotitie.transform.Find("FotoImage").GetComponent<RawImage>();
        Button fotoButton = nieuweNotitie.transform.Find("FotoButton").GetComponent<Button>();
        fotoButton.onClick.AddListener(() => OpenGallery(fotoImage));

        // Wis inputveld
        notitieInputField.text = "";
    }

    private void VerwijderNotitie(GameObject notitie)
    {
        Destroy(notitie);
    }

    private void OpenGallery(RawImage targetImage)
    {
#if UNITY_ANDROID || UNITY_IOS
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 512);
                if (texture != null)
                {
                    targetImage.texture = texture;
                    targetImage.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("Kon afbeelding niet laden.");
                }
            }
        }, "Kies een afbeelding");
#else
        Debug.Log("Foto's toevoegen werkt alleen op een mobiel toestel.");
#endif
    }
}
