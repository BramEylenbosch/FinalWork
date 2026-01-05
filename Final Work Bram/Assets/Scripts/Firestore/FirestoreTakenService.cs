using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirestoreTakenService
{
    FirebaseFirestore db => FirebaseFirestore.DefaultInstance;

    // Dynamische targetUserId: mantelzorger of gebruiker
    private string TargetUserId
    {
        get
        {
            if (UserContext.UserRole == "Mantelzorger")
                return UserContext.UserId;          // Mantelzorger schrijft naar eigen document
            else
                return UserContext.CaretakerId;    // Gebruiker leest van gekoppelde mantelzorger
        }
    }

    public async Task VoegTaakToe(Taak taak)
    {
        if (string.IsNullOrEmpty(TargetUserId))
        {
            Debug.LogWarning("[FirestoreTakenService] TargetUserId is leeg, taak niet opgeslagen.");
            return;
        }

        await db.Collection("gebruikers")
                .Document(TargetUserId)
                .Collection("taken")
                .Document(taak.id)
                .SetAsync(taak);

        Debug.Log($"[FirestoreTakenService] Taak toegevoegd aan {TargetUserId}: {taak.tekst}");
    }

    public async Task<List<Taak>> LaadTaken()
    {
        List<Taak> taken = new List<Taak>();

        if (string.IsNullOrEmpty(TargetUserId))
        {
            Debug.LogWarning("[FirestoreTakenService] TargetUserId is leeg, geen taken geladen.");
            return taken;
        }

        QuerySnapshot snapshot = await db.Collection("gebruikers")
                                         .Document(TargetUserId)
                                         .Collection("taken")
                                         .GetSnapshotAsync();

        foreach (var doc in snapshot.Documents)
        {
            taken.Add(doc.ConvertTo<Taak>());
        }

        Debug.Log($"[FirestoreTakenService] {taken.Count} taken geladen voor {TargetUserId}");
        return taken;
    }

    public async Task VerwijderTaak(string taakId)
    {
        if (string.IsNullOrEmpty(TargetUserId))
        {
            Debug.LogWarning("[FirestoreTakenService] TargetUserId is leeg, taak niet verwijderd.");
            return;
        }

        await db.Collection("gebruikers")
                .Document(TargetUserId)
                .Collection("taken")
                .Document(taakId)
                .DeleteAsync();

        Debug.Log($"[FirestoreTakenService] Taak verwijderd uit {TargetUserId}: {taakId}");
    }
}
