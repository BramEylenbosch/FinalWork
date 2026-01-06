using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirestoreHandleidingService
{
    public static FirestoreHandleidingService Instance { get; } = new FirestoreHandleidingService();

    private FirebaseFirestore db => FirebaseFirestore.DefaultInstance;
    private string GebruikerId => UserContext.UserId;

    public async Task VoegHandleidingToe(string handleidingId, string naam)
    {
        var data = new Dictionary<string, object>
        {
            { "naam", naam },
            { "fotoUrls", new List<string>() },
            { "aangemaaktDoor", UserContext.CaretakerId },
            { "createdAt", Timestamp.GetCurrentTimestamp() }
        };

        await db.Collection("gebruikers")
                .Document(GebruikerId)
                .Collection("handleidingen")
                .Document(handleidingId)
                .SetAsync(data);

        Debug.Log("[Firestore] Handleiding aangemaakt: " + naam);
    }

    // Voeg URL van geüploade foto toe
public async Task VoegFotoUrlToe(string handleidingId, string fotoUrl)
{
    if (string.IsNullOrEmpty(handleidingId) || string.IsNullOrEmpty(fotoUrl))
    {
        Debug.LogError("handleidingId of fotoUrl is null of leeg!");
        return;
    }

    var docRef = db.Collection("gebruikers")
                   .Document(GebruikerId)
                   .Collection("handleidingen")
                   .Document(handleidingId);

    // Voeg toe of merge
    await docRef.SetAsync(new Dictionary<string, object>
    {
        { "fotoUrls", new List<string> { fotoUrl } }
    }, SetOptions.MergeAll);

    Debug.Log("[Firestore] Foto URL toegevoegd: " + fotoUrl);
}


    // Laad handleidingen inclusief foto URLs
public async Task<List<HandleidingData>> LaadHandleidingen()
{
    QuerySnapshot snapshot = await db
        .Collection("gebruikers")
        .Document(GebruikerId)
        .Collection("handleidingen")
        .GetSnapshotAsync();

    List<HandleidingData> lijst = new();

    foreach (var doc in snapshot.Documents)
    {
        string naam = doc.ContainsField("naam") ? doc.GetValue<string>("naam") : "Onbekende handleiding";
        List<string> urls = doc.ContainsField("fotoUrls") ? doc.GetValue<List<string>>("fotoUrls") : new List<string>();

        HandleidingData h = new HandleidingData(naam)
        {
            fotoUrls = urls,
            id = doc.Id
        };

        lijst.Add(h);
    }

    return lijst;
}

}
