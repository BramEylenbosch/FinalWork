using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PhotoSelector : MonoBehaviour
{
    public MemoryGameManager gameManager; // sleep hier je MemoryGameManager in inspector

    // Aanroepen via UI Button
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
                gameManager.AddPersonalPhoto(newSprite);
            }
        }, "Kies een foto", "image/*");

        // wacht tot klaar
        while (NativeGallery.IsMediaPickerBusy())
            yield return null;
    }
    else
    {
        Debug.Log("Media picker is al bezig.");
    }
}

}
