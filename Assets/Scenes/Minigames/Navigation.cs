using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    public void back()
    {
        SceneManager.LoadScene("PvE Worlds");
    }
}
