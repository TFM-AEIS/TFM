using UnityEngine;
using UnityEngine.SceneManagement;

public class ThemeSelectionNavigation : MonoBehaviour
{
    public void goToWorld(int world)
    {
        PvEWorldSelection.currentWorld = world;
        SceneManager.LoadScene("PvE Worlds");
    }

    public void back()
    {
        SceneManager.LoadScene("Start Menu");
    }
}
