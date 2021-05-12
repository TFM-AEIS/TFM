using UnityEngine;
using UnityEngine.SceneManagement;

public class PvEWorldsNavigation : MonoBehaviour
{
    [SerializeField] GameObject[] worlds;

    void Awake()
    {
        this.LoadWorld(PvEWorldSelection.currentWorld);
    }

    private void LoadWorld(int index)
    {
        for (int i = 0; i < this.worlds.Length; i++)
        {
            this.worlds[i].SetActive(false);
        }

        PvEWorldSelection.currentWorld = index;
        this.worlds[index].SetActive(true);
    }

    public void Back()
    {
        SceneManager.LoadScene("Theme Selection");
    }
}
