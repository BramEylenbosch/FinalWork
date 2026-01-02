using Firebase.Firestore;
using Firebase.Auth;
using System.Threading.Tasks;

public class KoppelingService
{
    private FirebaseFirestore db => FirebaseFirestore.DefaultInstance;

    public async Task KoppelMantelzorgerAanGebruiker(string gebruikerId)
    {
        string mantelzorgerId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        // Opslaan bij gebruiker
        await db.Collection("gebruikers")
            .Document(gebruikerId)
            .Collection("profiel")
            .Document("koppeling")
            .SetAsync(new
            {
                gekoppeldeMantelzorgerId = mantelzorgerId
            });

        // Opslaan bij mantelzorger
        await db.Collection("mantelzorgers")
            .Document(mantelzorgerId)
            .SetAsync(new
            {
                gekoppeldeGebruikerId = gebruikerId
            });
    }
}
