using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class HandleidingManagerGebruiker : MonoBehaviour
{
    [Header("UI References")]
    public Transform handleidingListParent;
    public GameObject handleidingButtonPrefab;
    public GameObject handleidingListPanel;
    public HandleidingViewerGebruiker viewer;

    private List<HandleidingData> handleidingen = new();

    private async void Start()
    {
        FirestoreHandleidingService service = new();

        // 🔥 Laad handleidingen uit Firestore
        handleidingen = await service.LaadHandleidingen();

        foreach (var h in handleidingen)
        {
            // ⬇️ Download alle foto's
            foreach (string url in h.fotoUrls)
            {
                StartCoroutine(DownloadSprite(url, h));
            }

            MaakKnopVoorHandleiding(h);
        }

        if (handleidingListPanel != null)
            handleidingListPanel.SetActive(true);
    }

    private void MaakKnopVoorHandleiding(HandleidingData h)
    {
        GameObject knop = Instantiate(handleidingButtonPrefab, handleidingListParent);

        TextMeshProUGUI naamText = knop.GetComponentInChildren<TextMeshProUGUI>();
        if (naamText != null)
            naamText.text = h.naam;

        Button mainButton = knop.GetComponent<Button>();
        if (mainButton != null)
            mainButton.onClick.AddListener(() => OpenHandleiding(h));

        HandleidingButtonController controller = knop.GetComponent<HandleidingButtonController>();
        if (controller != null && controller.verwijderKnop != null)
            controller.verwijderKnop.gameObject.SetActive(false);
    }

    private void OpenHandleiding(HandleidingData data)
    {
        if (viewer != null)
        {
            viewer.ToonHandleiding(data);
            if (handleidingListPanel != null)
                handleidingListPanel.SetActive(false);
        }
    }

    IEnumerator DownloadSprite(string url, HandleidingData handleiding)
    {
        using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[DownloadSprite] Fout: " + www.error);
            yield break;
        }

        Texture2D tex = DownloadHandlerTexture.GetContent(www);
        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f)
        );

        handleiding.fotos.Add(sprite);
    }
}
