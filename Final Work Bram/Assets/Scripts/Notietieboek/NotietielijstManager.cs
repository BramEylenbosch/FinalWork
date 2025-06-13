using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NotitieLijstManager : MonoBehaviour
{
    [Header("UI Containers")]
    public Transform notitieContainer;            // ScrollView > Content
    public GameObject notitiePrefab;              // Prefab met TMP_Text (NotitieTekst), DeleteButton, DatumText, FotoImage

    [Header("Popup Elements")]
    public GameObject popupPanel;
    public TMP_InputField popupTekstInput;
    public TMP_InputField popupDatumInput;
    public RawImage popupFotoImage;
    public Button popupFotoKiesButton;
    public Button popupToevoegenButton;
    public Button popupAnnulerenButton;

    [Header("Hoofdinterface")]
    public GameObject scrollViewPanel;            // Hele scrollview of de Content parent
    public GameObject nieuweNotitieKnop;          // De "Nieuwe Notitie"-knop
    public GameObject hoofdtekstObject;

    private NotitieDataLijst notitieDataLijst = new NotitieDataLijst();
    private string gekozenFotoPad = "";

    private const float yStart = 95f;
    private const float ySpacing = 195f;

    private void Start()
    {
        popupPanel.SetActive(false);
        popupFotoKiesButton.onClick.AddListener(KiesFoto);
        popupToevoegenButton.onClick.AddListener(VoegNotitieVanPopupToe);
        popupAnnulerenButton.onClick.AddListener(SluitPopup);

        LaadNotities();
    }

    public void OpenNotitiePopup()
    {
        popupTekstInput.text = "";
        popupDatumInput.text = "";
        gekozenFotoPad = "";
        popupFotoImage.texture = null;
        popupFotoImage.gameObject.SetActive(false);

        popupPanel.SetActive(true);
        scrollViewPanel.SetActive(false);
        nieuweNotitieKnop.SetActive(false);
        hoofdtekstObject.SetActive(false);
    }

    private void SluitPopup()
    {
        popupPanel.SetActive(false);
        scrollViewPanel.SetActive(true);
        nieuweNotitieKnop.SetActive(true);
        hoofdtekstObject.SetActive(true);
    }

    private void KiesFoto()
    {
#if UNITY_ANDROID || UNITY_IOS
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                gekozenFotoPad = path;

                Texture2D texture = NativeGallery.LoadImageAtPath(path, 512);
                if (texture != null)
                {
                    popupFotoImage.texture = texture;
                    popupFotoImage.gameObject.SetActive(true);
                }
            }
        }, "Kies een afbeelding");
#endif
    }

    private void VoegNotitieVanPopupToe()
    {
        string tekst = popupTekstInput.text;
        if (string.IsNullOrWhiteSpace(tekst)) return;

        NotitieData nieuweData = new NotitieData
        {
            tekst = tekst,
            datum = popupDatumInput.text,
            fotoPad = gekozenFotoPad
        };

        notitieDataLijst.notities.Add(nieuweData);
        SlaNotitiesOp();
        MaakNotitieUI(nieuweData);
        SluitPopup();
    }

    private void MaakNotitieUI(NotitieData data)
    {
        GameObject nieuweNotitie = Instantiate(notitiePrefab, notitieContainer, false);
        int index = notitieContainer.childCount - 1;

        RectTransform rt = nieuweNotitie.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, yStart - index * ySpacing);

        // Zoek het TMP_Text component in de prefab child "NotitieTekst"
        TMP_Text notitieTekst = nieuweNotitie.transform.Find("NotitieTekst")?.GetComponent<TMP_Text>();

        if (notitieTekst != null)
        {
            Debug.Log("NotitieTekst wordt gezet op: " + data.tekst);
            notitieTekst.text = string.IsNullOrEmpty(data.tekst) ? "" : data.tekst;
        }
        else
        {
            Debug.LogWarning("NotitieTekst component niet gevonden in prefab!");
        }

        TMP_Text datumText = nieuweNotitie.transform.Find("DatumText")?.GetComponent<TMP_Text>();
        if (datumText != null) datumText.text = data.datum;

        RawImage fotoImage = nieuweNotitie.transform.Find("FotoImage")?.GetComponent<RawImage>();
        if (!string.IsNullOrEmpty(data.fotoPad) && fotoImage != null)
        {
            Texture2D texture = NativeGallery.LoadImageAtPath(data.fotoPad, 512);
            if (texture != null)
            {
                fotoImage.texture = texture;
                fotoImage.gameObject.SetActive(true);
            }
        }

        Button deleteButton = nieuweNotitie.transform.Find("DeleteButton").GetComponent<Button>();
        deleteButton.onClick.AddListener(() =>
        {
            VerwijderNotitie(nieuweNotitie, data);
        });

        PasContainerHoogteAan();
    }

    private void VerwijderNotitie(GameObject notitie, NotitieData data)
    {
        Destroy(notitie);
        notitieDataLijst.notities.Remove(data);
        SlaNotitiesOp();
        StartCoroutine(HerpositioneerNotities());
    }

    private IEnumerator HerpositioneerNotities()
    {
        yield return null; // wacht 1 frame
        int index = 0;
        foreach (Transform child in notitieContainer)
        {
            RectTransform rt = child.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, yStart - index * ySpacing);
            index++;
        }
        PasContainerHoogteAan();
    }

    private void PasContainerHoogteAan()
    {
        RectTransform containerRT = notitieContainer.GetComponent<RectTransform>();
        float hoogte = notitieDataLijst.notities.Count * ySpacing + 200;
        containerRT.sizeDelta = new Vector2(containerRT.sizeDelta.x, hoogte);
    }

    private void SlaNotitiesOp()
    {
        string json = JsonUtility.ToJson(notitieDataLijst);
        PlayerPrefs.SetString("notities", json);
        PlayerPrefs.Save();
    }

    private void LaadNotities()
    {
        if (!PlayerPrefs.HasKey("notities")) return;

        string json = PlayerPrefs.GetString("notities");
        notitieDataLijst = JsonUtility.FromJson<NotitieDataLijst>(json);

        foreach (var data in notitieDataLijst.notities)
        {
            MaakNotitieUI(data);
        }
    }
}

[System.Serializable]
public class NotitieData
{
    public string tekst;
    public string datum;
    public string fotoPad;
}

[System.Serializable]
public class NotitieDataLijst
{
    public List<NotitieData> notities = new List<NotitieData>();
}
