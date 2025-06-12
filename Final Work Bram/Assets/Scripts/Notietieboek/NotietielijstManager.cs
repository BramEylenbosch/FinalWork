using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NotitieLijstManager : MonoBehaviour
{
    public Transform notitieContainer;         // ScrollView > Content
    public GameObject notitiePrefab;           // Prefab met InputField, Verwijderknop, Fotoknop & RawImage
    public TMP_InputField notitieInputField;   // Input bovenaan voor nieuwe notitie

    public float hoogteNotitie = 100f;         // Hoogte van een notitie voor container berekening

    public void VoegNotitieToe()
    {
        string tekst = notitieInputField.text;
        if (string.IsNullOrWhiteSpace(tekst)) return;

        GameObject nieuweNotitie = Instantiate(notitiePrefab, notitieContainer, false);

        int index = notitieContainer.childCount - 1; // index van nieuwe notitie
        float yPos = GetYPosForIndex(index);

        RectTransform rt = nieuweNotitie.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, yPos);

        TMP_InputField inputField = nieuweNotitie.GetComponentInChildren<TMP_InputField>();
        inputField.text = tekst;

        Button deleteButton = nieuweNotitie.transform.Find("DeleteButton").GetComponent<Button>();
        deleteButton.onClick.AddListener(() => VerwijderNotitie(nieuweNotitie));

        RawImage fotoImage = nieuweNotitie.transform.Find("FotoImage").GetComponent<RawImage>();
        Button fotoButton = nieuweNotitie.transform.Find("FotoButton").GetComponent<Button>();
        fotoButton.onClick.AddListener(() => OpenGallery(fotoImage));

        notitieInputField.text = "";

        PasContainerHoogteAan();
    }

    private void VerwijderNotitie(GameObject notitie)
    {
        Destroy(notitie);
        StartCoroutine(HerpositioneerNotities());
    }

    private IEnumerator HerpositioneerNotities()
    {
        yield return new WaitForEndOfFrame();

        int index = 0;
        foreach (Transform child in notitieContainer)
        {
            RectTransform rt = child.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, GetYPosForIndex(index));
            index++;
        }

        PasContainerHoogteAan();
    }

    private float GetYPosForIndex(int index)
    {
        if (index == 0) return 175f;
        if (index == 1) return -20f;

        float afstand = 195f;
        return -20f - (index - 1) * afstand;
    }

    private void PasContainerHoogteAan()
    {
        RectTransform containerRT = notitieContainer.GetComponent<RectTransform>();
        int aantalNotities = notitieContainer.childCount;

        if (aantalNotities == 0)
            return;

        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int i = 0; i < aantalNotities; i++)
        {
            float y = GetYPosForIndex(i);
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }

        float hoogteNodig = maxY - minY + hoogteNotitie;
        hoogteNodig = Mathf.Max(hoogteNodig, containerRT.rect.height);

        containerRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, hoogteNodig);
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
