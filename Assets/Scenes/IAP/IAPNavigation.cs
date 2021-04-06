using UnityEngine;
using UnityEngine.SceneManagement;

public class IAPNavigation : MonoBehaviour
{

    public void back()
    {
        SceneManager.LoadScene("Start Menu");
    }
}
