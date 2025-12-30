using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirestoreTakenService
{
    FirebaseFirestore db => FirebaseFirestore.DefaultInstance;
    string GebruikerId => PlayerPrefs.GetString("gebruikerId");

    public Task VoegTaakToe(Taak taak)
    {
        return db.Collection("gebruikers")
                 .Document(GebruikerId)
                 .Collection("taken")
                 .Document(taak.id)
                 .SetAsync(taak);
    }

    public async Task<List<Taak>> LaadTaken()
    {
        QuerySnapshot snapshot = await db
            .Collection("gebruikers")
            .Document(GebruikerId)
            .Collection("taken")
            .GetSnapshotAsync();

        List<Taak> taken = new();

        foreach (var doc in snapshot.Documents)
            taken.Add(doc.ConvertTo<Taak>());

        return taken;
    }

    public Task VerwijderTaak(string taakId)
    {
        return db.Collection("gebruikers")
                 .Document(GebruikerId)
                 .Collection("taken")
                 .Document(taakId)
                 .DeleteAsync();
    }
}
