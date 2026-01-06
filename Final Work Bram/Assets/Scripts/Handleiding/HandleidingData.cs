using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class HandleidingData
{
    public string id; // unieke id van de handleiding
    public string naam;
    public List<Sprite> fotos;      // lokaal gebruik
    public List<string> fotoUrls;   // Firebase opslag
    public List<string> videoUrls = new List<string>();

    public HandleidingData(string naam)
    {
        this.naam = naam;
        this.fotos = new List<Sprite>();
        this.fotoUrls = new List<string>();
        this.id = System.Guid.NewGuid().ToString();
    }
}

