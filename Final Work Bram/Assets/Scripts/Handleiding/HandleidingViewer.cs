using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

public class HandleidingViewer : MonoBehaviour
{
    [Header("UI")]
    public Image paginaImage;
    public Button vorigeKnop;
    public Button volgendeKnop;
    public Button fotoToevoegenKnop;
    public Button sluitKnop;

    [HideInInspector] public HandleidingManager manager;

    private HandleidingData huidigeHandleiding;
    private int huidigeIndex = 0;

    void Start()
    {
        vorigeKnop.onClick.AddListener(Vorige);
        volgendeKnop.onClick.AddListener(Volgende);
        fotoToevoegenKnop.onClick.RemoveAllListeners();
        fotoToevoegenKnop.onClick.AddListener(NeemFoto);
        sluitKnop.onClick.AddListener(SluitViewer);

        // Zet standaard verborgen
        gameObject.SetActive(false);

        // Alleen tonen als mantelzorger scene
        if (manager == null)
            fotoToevoegenKnop.gameObject.SetActive(false);
    }

public void ToonHandleiding(HandleidingData data, bool isMantelzorger)
{
    huidigeHandleiding = data;
    huidigeIndex = 0;
    gameObject.SetActive(true);

    // Alleen tonen als mantelzorger
    fotoToevoegenKnop.gameObject.SetActive(isMantelzorger);

    // Start downloaden van foto's
    if (huidigeHandleiding.fotoUrls != null && huidigeHandleiding.fotoUrls.Count > 0)
        StartCoroutine(DownloadFotos(huidigeHandleiding));
    else
        ToonPagina();
}



    private void ToonPagina()
    {
        if (huidigeHandleiding == null || paginaImage == null)
            return;

        // Toon placeholder als er nog geen foto is
        if (huidigeHandleiding.fotos.Count == 0)
        {
            paginaImage.sprite = null;
            paginaImage.color = Color.gray; // lege placeholder
            vorigeKnop.interactable = false;
            volgendeKnop.interactable = false;
            return;
        }

        // Toon de huidige foto
        paginaImage.sprite = huidigeHandleiding.fotos[huidigeIndex];
        paginaImage.color = Color.white;

        vorigeKnop.interactable = huidigeIndex > 0;
        volgendeKnop.interactable = huidigeIndex < huidigeHandleiding.fotos.Count - 1;
    }

    public void Volgende()
    {
        if (huidigeHandleiding == null || huidigeHandleiding.fotos.Count == 0) return;
        if (huidigeIndex < huidigeHandleiding.fotos.Count - 1)
        {
            huidigeIndex++;
            ToonPagina();
        }
    }

    public void Vorige()
    {
        if (huidigeHandleiding == null || huidigeHandleiding.fotos.Count == 0) return;
        if (huidigeIndex > 0)
        {
            huidigeIndex--;
            ToonPagina();
        }
    }

    public void NeemFoto()
    {
        NativeCamera.TakePicture((path) =>
        {
            if (path != null)
            {
                Texture2D texture = NativeCamera.LoadImageAtPath(path, 1024, false);
                if (texture != null)
                    FotoToevoegen(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)));
            }
        }, maxSize: 1024);
    }

public async void FotoToevoegen(Sprite nieuweFoto)
{
    if (huidigeHandleiding == null || nieuweFoto == null)
    {
        Debug.LogError("[HandleidingViewer] Geen handleiding geselecteerd of foto is null!");
        return;
    }
    if (string.IsNullOrEmpty(huidigeHandleiding?.id))
    {
        Debug.LogError("[HandleidingViewer] Kan foto niet uploaden: handleidingId is null!");
        return;
    }

// Voeg foto toe lokaal voor directe weergave
huidigeHandleiding.fotos.Add(nieuweFoto); // ✅ keep alle bestaande foto's
huidigeIndex = huidigeHandleiding.fotos.Count - 1;
ToonPagina();


    // Upload de foto naar Firebase Storage
    Texture2D tex = nieuweFoto.texture;

    if (tex == null)
    {
        Debug.LogError("[HandleidingViewer] Sprite heeft geen texture!");
        return;
    }

    string handleidingId = huidigeHandleiding.id;
    if (string.IsNullOrEmpty(handleidingId))
    {
        Debug.LogError("[HandleidingViewer] handleidingId is null of leeg!");
        return;
    }

    try
    {
        // Upload naar Firebase Storage en krijg de download URL
        string fotoUrl = await FirebaseStorageService.Instance.UploadFoto(tex, handleidingId);

        if (!string.IsNullOrEmpty(fotoUrl))
        {
            // Voeg de URL toe in Firestore
            await FirestoreHandleidingService.Instance.VoegFotoUrlToe(handleidingId, fotoUrl);

            // Ook lokaal opslaan in HandleidingData
            if (huidigeHandleiding.fotoUrls == null)
                huidigeHandleiding.fotoUrls = new List<string>();
            huidigeHandleiding.fotoUrls.Add(fotoUrl);

            Debug.Log("[HandleidingViewer] Foto succesvol geüpload en opgeslagen in Firestore!");
        }
    }
    catch (Exception ex)
    {
        Debug.LogError("[HandleidingViewer] Fout bij upload: " + ex);
    }
}



    public void SluitViewer()
    {
        gameObject.SetActive(false);

        if (manager != null && manager.handleidingListPanel != null)
            manager.handleidingListPanel.SetActive(true);
    }

    // Getter voor huidige handleiding
    public HandleidingData HuidigeHandleiding => huidigeHandleiding;
    public async void VoegFotoToeAanHandleiding(Sprite nieuweFoto)
{
    if (huidigeHandleiding == null || nieuweFoto == null) return;

    Texture2D tex = nieuweFoto.texture;

    // Upload naar Firebase Storage
    string fotoUrl = await FirebaseStorageService.Instance.UploadFoto(tex, huidigeHandleiding.id);

    // Voeg URL toe aan Firestore
    await FirestoreHandleidingService.Instance.VoegFotoUrlToe(huidigeHandleiding.id, fotoUrl);

    // Voeg lokaal toe
    huidigeHandleiding.fotos.Add(nieuweFoto);
    huidigeHandleiding.fotoUrls.Add(fotoUrl);

    // Sla lokaal op
    DataOpslagSystem.SlaHandleidingenOp(manager.GetHandleidingen());

    // Update de viewer
    ToonPagina();
}

public IEnumerator DownloadFotos(HandleidingData handleiding)
{
    if (handleiding.fotos == null)
        handleiding.fotos = new List<Sprite>();

    foreach (string url in handleiding.fotoUrls)
    {
        bool alGedownload = handleiding.fotos.Exists(f => f.name == url);
        if (alGedownload) continue;

        using var www = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.LogError("Fout bij downloaden: " + www.error);
            continue;
        }

        Texture2D tex = UnityEngine.Networking.DownloadHandlerTexture.GetContent(www);
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        sprite.name = url; // ⚡ naam van sprite = url
        handleiding.fotos.Add(sprite);
    }

    ToonPagina();
}




}
