using UnityEngine;
using UnityEngine.SceneManagement;

public class PvEWorldsNavigation : MonoBehaviour
{
    [SerializeField] GameObject[] worlds;

    void Awake()
    {
        this.loadWorld(PvEWorldSelection.currentWorld);
    }

    private void loadWorld(int index)
    {
        for (int i = 0; i < this.worlds.Length; i++)
        {
            this.worlds[i].SetActive(false);
        }

        PvEWorldSelection.currentWorld = index;
        this.worlds[index].SetActive(true);
    }

    public void back()
    {
        SceneManager.LoadScene("Theme Selection");
    }
}
