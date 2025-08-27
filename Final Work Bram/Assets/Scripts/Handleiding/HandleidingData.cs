using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleidingData
{
    public string naam;
    public List<Sprite> fotos = new List<Sprite>();

    public HandleidingData(string naam)
    {
        this.naam = naam;
    }
}
