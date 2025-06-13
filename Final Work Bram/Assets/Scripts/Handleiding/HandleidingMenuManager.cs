using UnityEngine;

public class HandleidingMenuManager : MonoBehaviour
{
    public GameObject hoofdmenuPanel;
    public GameObject microgolfPanel;
    public GameObject koffiePanel;
    public GameObject vaatwasserPanel;
    

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
    public void OpenVaatwasser()
    {
        hoofdmenuPanel.SetActive(false);
        vaatwasserPanel.SetActive(true);
    }

    public void TerugNaarMenu()
    {
        hoofdmenuPanel.SetActive(true);
        microgolfPanel.SetActive(false);
        koffiePanel.SetActive(false);
        vaatwasserPanel.SetActive(false);
    }
}
