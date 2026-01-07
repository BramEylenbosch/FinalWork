using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;
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
        sluitKnop.onClick.AddListener(SluitViewer);


        fotoToevoegenKnop.onClick.RemoveAllListeners();
        fotoToevoegenKnop.onClick.AddListener(() => NeemFoto());


        gameObject.SetActive(false);
    }


public void ToonHandleiding(HandleidingData data)
{
    huidigeHandleiding = data;
    huidigeIndex = 0;
    gameObject.SetActive(true);


fotoToevoegenKnop.gameObject.SetActive(manager != null &&
                                       manager.gebruikerType == HandleidingManager.GebruikerType.Mantelzorger);


    if (huidigeHandleiding.fotoUrls != null && huidigeHandleiding.fotoUrls.Count > 0)
        StartCoroutine(DownloadFotos(huidigeHandleiding));
    else
        ToonPagina();
}


    private void ToonPagina()
    {
        if (huidigeHandleiding == null || paginaImage == null)
            return;

        if (huidigeHandleiding.fotos.Count == 0)
        {
            paginaImage.sprite = null;
            paginaImage.color = Color.gray; 
            vorigeKnop.interactable = false;
            volgendeKnop.interactable = false;
            return;
        }

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
            if (string.IsNullOrEmpty(path)) return;

            Texture2D tex = NativeCamera.LoadImageAtPath(path, 1024, false);
            if (tex != null)
            {
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                StartCoroutine(FotoToevoegenAsync(sprite));
            }
        }, maxSize: 1024);
    }


    private IEnumerator FotoToevoegenAsync(Sprite nieuweFoto)
    {
        if (huidigeHandleiding == null || nieuweFoto == null)
        {
            Debug.LogError("[HandleidingViewer] Geen handleiding geselecteerd of foto is null!");
            yield break;
        }


        huidigeHandleiding.fotos.Add(nieuweFoto);
        huidigeIndex = huidigeHandleiding.fotos.Count - 1;
        ToonPagina();


        Texture2D tex = nieuweFoto.texture;
        Task<string> uploadTask = FirebaseStorageService.Instance.UploadFoto(tex, huidigeHandleiding.id);

        yield return new WaitUntil(() => uploadTask.IsCompleted);

        if (uploadTask.IsFaulted)
        {
            Debug.LogError("[HandleidingViewer] Fout bij upload: " + uploadTask.Exception);
            yield break;
        }

        string fotoUrl = uploadTask.Result;

        if (!string.IsNullOrEmpty(fotoUrl))
        {

            Task firestoreTask = FirestoreHandleidingService.Instance.VoegFotoUrlToe(huidigeHandleiding.id, fotoUrl);
            yield return new WaitUntil(() => firestoreTask.IsCompleted);


            if (huidigeHandleiding.fotoUrls == null)
                huidigeHandleiding.fotoUrls = new List<string>();

            if (!huidigeHandleiding.fotoUrls.Contains(fotoUrl))
                huidigeHandleiding.fotoUrls.Add(fotoUrl);

            DataOpslagSystem.SlaHandleidingenOp(manager.GetHandleidingen());

            Debug.Log("[HandleidingViewer] Foto succesvol geüpload en opgeslagen!");
        }
    }


public IEnumerator DownloadFotos(HandleidingData handleiding)
{
    if (handleiding.fotoUrls == null || handleiding.fotoUrls.Count == 0)
    {
        ToonPagina();
        yield break;
    }


    HashSet<string> bestaandeUrls = new HashSet<string>(handleiding.fotoUrls);

    List<Sprite> nieuweFotos = new List<Sprite>();

    foreach (string url in handleiding.fotoUrls)
    {

        bool bestaatAl = handleiding.fotos.Exists(f => f.name == GetNameFromUrl(url));
        if (bestaatAl)
            continue;

        using var www = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.LogError("[DownloadFotos] Fout bij downloaden: " + www.error);
            continue;
        }

        Texture2D tex = UnityEngine.Networking.DownloadHandlerTexture.GetContent(www);
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        {
            name = GetNameFromUrl(url);
        };

        nieuweFotos.Add(sprite);
    }


    handleiding.fotos.AddRange(nieuweFotos);


    ToonPagina();
}


private string GetNameFromUrl(string url)
{
    if (string.IsNullOrEmpty(url))
        return Guid.NewGuid().ToString(); 

    string[] parts = url.Split('/');
    return parts.Length > 0 ? parts[^1] : Guid.NewGuid().ToString();
}


    public void SluitViewer()
    {
        gameObject.SetActive(false);

        if (manager != null && manager.handleidingListPanel != null)
            manager.handleidingListPanel.SetActive(true);
    }

    public HandleidingData HuidigeHandleiding => huidigeHandleiding;
}
