using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhotoSelector : MonoBehaviour
{
    public MemoryGameManager gameManager;

    [Header("Popup Inputs")]
    public TMP_InputField naamInput;
    public TMP_InputField functieInput;

    public void PickPhotoFromGallery()
    {
        StartCoroutine(PickImageRoutine());
    }

    private IEnumerator PickImageRoutine()
    {
        if (!NativeGallery.IsMediaPickerBusy())
        {
            NativeGallery.GetImageFromGallery((path) =>
            {
                if (path != null)
                {
                    Texture2D texture = NativeGallery.LoadImageAtPath(path, 512);
                    if (texture == null)
                    {
                        Debug.LogWarning("Kon afbeelding niet laden.");
                        return;
                    }

                    Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

                    // Voeg de foto toe met naam + functie
                    VoegPersonalPhotoToe(newSprite);
                }
            }, "Kies een foto", "image/*");

            while (NativeGallery.IsMediaPickerBusy())
                yield return null;
        }
        else
        {
            Debug.Log("Media picker is al bezig.");
        }
    }

    private void VoegPersonalPhotoToe(Sprite photo)
    {
        string naam = naamInput.text.Trim();
        string functie = functieInput.text.Trim();

        if (string.IsNullOrEmpty(naam)) naam = "";

        PersonalPhotoData data = new PersonalPhotoData(photo, naam, functie);
        gameManager.AddPersonalPhotoData(data);

        // Reset input fields na toevoegen
        naamInput.text = "";
        functieInput.text = "";
    }
}
