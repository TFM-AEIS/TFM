using UnityEngine;

public class PvEWorldSelection : MonoBehaviour
{
    public static PvEWorldSelection GM;

    public static int currentWorld = 1;

    void Awake()
    {
        if (GM != null)
            GameObject.Destroy(GM);
        else
            GM = this;

        DontDestroyOnLoad(this);
    }
}
