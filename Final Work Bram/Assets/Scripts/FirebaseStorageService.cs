using System;
using System.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;

public class FirebaseStorageService
{
    private static FirebaseStorageService _instance;
    public static FirebaseStorageService Instance => _instance ??= new FirebaseStorageService();

    private FirebaseStorage storage;

    private FirebaseStorageService()
    {
        storage = FirebaseStorage.DefaultInstance;
    }

    public async Task<string> UploadFoto(Texture2D texture, string handleidingId, string bestandsnaam = null)
    {
        if (texture == null)
        {
            Debug.LogError("Texture is null!");
            return null;
        }

        if (string.IsNullOrEmpty(handleidingId))
        {
            Debug.LogError("handleidingId is null of leeg!");
            return null;
        }

        bestandsnaam ??= System.Guid.NewGuid().ToString() + ".png";

        byte[] bytes = texture.EncodeToPNG();
        StorageReference refFoto = storage.GetReference($"handleidingen/{handleidingId}/{bestandsnaam}");

        await refFoto.PutBytesAsync(bytes);

        var uri = await refFoto.GetDownloadUrlAsync();

        string downloadUrl = uri.AbsoluteUri;

        Debug.Log("[FirebaseStorage] Foto geüpload: " + downloadUrl);

        return downloadUrl;
    }

}
