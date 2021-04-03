using UnityEngine;
using UnityEngine.SceneManagement;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField] private Canvas main;
    [SerializeField] private Canvas pve;
    [SerializeField] private Canvas pvp;
    [SerializeField] private Canvas clan;
    [SerializeField] private Canvas settings;

    private void Start()
    {
        goToMain();
    }

    public void goToMain()
    {
        main.gameObject.SetActive(true);
        pve.gameObject.SetActive(false);
        pvp.gameObject.SetActive(false);
        clan.gameObject.SetActive(false);
        settings.gameObject.SetActive(false);
    }
    public void goToPvE()
    {
        main.gameObject.SetActive(false);
        pve.gameObject.SetActive(true);
    }

    public void goToPvP()
    {
        main.gameObject.SetActive(false);
        pvp.gameObject.SetActive(true);
    }

    public void goToClan()
    {
        main.gameObject.SetActive(false);
        clan.gameObject.SetActive(true);
    }

    public void goToSettings()
    {
        main.gameObject.SetActive(false);
        settings.gameObject.SetActive(true);
    }

    public void goToIAP()
    {
        SceneManager.LoadScene("IAP");
    }
}
