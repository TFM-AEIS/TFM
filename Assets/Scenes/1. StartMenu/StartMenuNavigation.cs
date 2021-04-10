using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuNavigation : MonoBehaviour
{
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject pvp;
    [SerializeField] private GameObject clan;
    [SerializeField] private GameObject settings;

    private void Start()
    {
        this.goToMain();
    }

    public void goToMain()
    {
        this.main.SetActive(true);
        this.pvp.SetActive(false);
        this.clan.SetActive(false);
        this.settings.SetActive(false);
    }

    public void goToPvE()
    {
        SceneManager.LoadScene("Theme Selection");
    }

    public void goToPvP()
    {
        this.main.SetActive(false);
        this.pvp.SetActive(true);
    }

    public void goToClan()
    {
        this.main.SetActive(false);
        this.clan.SetActive(true);
    }

    public void goToSettings()
    {
        this.settings.SetActive(true);
    }

    public void goToIAP()
    {
        SceneManager.LoadScene("IAP");
    }
}
