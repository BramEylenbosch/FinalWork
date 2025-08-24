using UnityEngine;

[System.Serializable]
public class PersonalPhotoData
{
    public Sprite photo;
    public string naam;
    public string functie;

    public PersonalPhotoData(Sprite sprite, string n, string f)
    {
        photo = sprite;
        naam = n;
        functie = f;
    }
}
