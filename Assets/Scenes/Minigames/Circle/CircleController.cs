using UnityEngine;

public class CircleController : MonoBehaviour
{
    [SerializeField] private GameObject movingCircumference;
    [SerializeField] private GameObject scoreCircumference;

    public static bool playing = false;

    private void Update()
    {
        if (playing && Input.GetButtonUp("Fire1"))
        {
            float val1 = movingCircumference.GetComponent<LineRenderer>().GetPosition(0).x;
            float val2 = scoreCircumference.GetComponent<LineRenderer>().GetPosition(0).x;
            Debug.Log("Score: " + ((3 - Mathf.Abs(val2 - val1)) * 10 / 3).ToString("F2") + "/10");
        }

        if (Input.GetButton("Fire1")) playing = true;
        else playing = false;
    }
}
