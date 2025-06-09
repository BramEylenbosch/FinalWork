using UnityEngine;

public class HandleidingMenuManager : MonoBehaviour
{
    public GameObject hoofdmenuPanel;
    public GameObject microgolfPanel;
    public GameObject koffiePanel;
    // Voeg meer panelen toe als je meer apparaten hebt

    public void OpenMicrogolf()
    {
        hoofdmenuPanel.SetActive(false);
        microgolfPanel.SetActive(true);
    }

    public void OpenKoffie()
    {
        hoofdmenuPanel.SetActive(false);
        koffiePanel.SetActive(true);
    }

    public void TerugNaarMenu()
    {
        hoofdmenuPanel.SetActive(true);
        microgolfPanel.SetActive(false);
        koffiePanel.SetActive(false);
        // Zet hier ook andere panels uit als je er meer hebt
    }
}
