using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleidingData
{
    public string naam;
    public List<string> fotosBase64;

    [System.NonSerialized]
    public List<Sprite> fotos;

    public HandleidingData(string naam)
    {
        this.naam = naam;
        fotosBase64 = new List<string>();
        fotos = new List<Sprite>(); // Altijd initialiseren!
    }
}
