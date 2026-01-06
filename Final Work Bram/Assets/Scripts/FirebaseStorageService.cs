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

    // Upload een foto naar Firebase Storage
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

        // Upload de bytes
        await refFoto.PutBytesAsync(bytes);

        // Download URL ophalen
        var uri = await refFoto.GetDownloadUrlAsync();

        // ⚡ Belangrijk: converteer Uri naar string
        string downloadUrl = uri.AbsoluteUri;

        Debug.Log("[FirebaseStorage] Foto geüpload: " + downloadUrl);

        return downloadUrl;
    }

}
