using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CompareSymmetry : MonoBehaviour
{
    private Texture2D t1;

    void Start()
    {
        t1 = GetComponent<SpriteRenderer>().sprite.texture;
        CalculateSymmetryScore();
    }

    private void CalculateSymmetryScore()
    {
        Color32[] colors = t1.GetPixels32();

        int score = 0;

        int side = t1.width;
        int halfside = side / 2;

        for (int i = 0; i < side; i++)
        {
            for (int j = 0; j < halfside; j++)
            {
                if (CompareColor32(colors[i * side + j], colors[(i + 1) * side - j - 1]))
                {
                    score++;
                }
            }
        }

        Debug.Log("Score: " + score + " / " + (side * halfside));
    }

    private bool CompareColor32(Color32 c1, Color32 c2)
    {
        int tolerance = 15;

        if (Mathf.Abs(c1.r - c2.r) > tolerance) return false;
        if (Mathf.Abs(c1.g - c2.g) > tolerance) return false;
        if (Mathf.Abs(c1.b - c2.b) > tolerance) return false;
        if (Mathf.Abs(c1.a - c2.a) > tolerance) return false;

        return true;
    }
}
