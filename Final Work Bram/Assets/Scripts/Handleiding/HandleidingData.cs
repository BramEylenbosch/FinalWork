using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleidingData
{
    public string id; // unieke id van de handleiding
    public string naam;
    public List<Sprite> fotos;      // lokaal gebruik
    public List<string> fotoUrls;   // Firebase opslag

    public HandleidingData(string naam)
    {
        this.naam = naam;
        this.fotos = new List<Sprite>();
        this.fotoUrls = new List<string>();
        this.id = System.Guid.NewGuid().ToString();
    }
}

