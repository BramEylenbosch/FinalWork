using System;
using Firebase.Firestore;

[Serializable]
public class Taak
{
    public string id;
    public string tekst;
    public string deadline;
    public bool voltooid;
}
