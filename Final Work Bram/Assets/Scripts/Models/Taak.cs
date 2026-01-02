using Firebase.Firestore;
using System;

[FirestoreData]
[Serializable]
public class Taak
{
    [FirestoreProperty]
    public string id { get; set; }

    [FirestoreProperty]
    public string tekst { get; set; }

    [FirestoreProperty]
    public string deadline { get; set; }

    [FirestoreProperty]
    public bool voltooid { get; set; }
}
