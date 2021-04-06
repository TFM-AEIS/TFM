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
        this.goFromTo(this.main, this.pvp);
    }

    public void goToClan()
    {
        this.goFromTo(this.main, this.clan);
    }

    public void goToSettings()
    {
        this.goFromTo(this.main, this.settings);
    }

    public void goToIAP()
    {
        SceneManager.LoadScene("IAP");
    }

    ////////

    public void goFromTo(GameObject from, GameObject to)
    {
        from.SetActive(false);
        to.SetActive(true);
    }
}
