using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CompareSymmetry : MonoBehaviour
{
    // Podemos tener en cuenta si el jugador es zurdo o diestro, para el lado en el que debe dibujar
    // private bool isPlayerLeftie = false;

    private Texture2D t1;

    void Start()
    {
        t1 = GetComponent<SpriteRenderer>().sprite.texture;
        CalculateSymmetryScoreByProxy(2); // Usar 2 (2*2 = 4 pixeles) o 4 (4*4 = 16 píxeles)
    }

    public int CalculateSymmetryScoreByProxy(int cellSide)
    {
        Color32[] colors = t1.GetPixels32();

        int score = 0;

        int side = t1.width;
        int halfside = side / 2;

        for (int i = 0; i < side; i += cellSide)
        {
            for (int j = 0; j < halfside; j += cellSide)
            {
                int matches = 0;

                for (int k = i; k < i + cellSide; k++)
                {
                    for (int l = j; l < j + cellSide; l++)
                    {
                        if (CompareColor32(colors[k * side + l], colors[(k + 1) * side - l - 1], 15))
                        {
                            matches++;
                        }
                        else // marca pixeles no simétricos en rojo para DebugTexture
                        {
                            colors[k * side + l] = new Color32(255, 0, 0, 255);
                            colors[(k + 1) * side - l - 1] = new Color32(255, 0, 0, 255);
                            //Debug.Log("Color1: " + colors[i * side + j] + ", Color2: " + colors[(i + 1) * side - j - 1]);
                        }
                    }
                }

                // si hay +50% de simetría en este área
                if (matches >= cellSide)
                {
                    score++;
                }
            }
        }

        //---------------------------------------------//
        // DEBUG
        //---------------------------------------------//

        Debug.Log("Score: " + score + " / " + (colors.Length / (cellSide * cellSide * 2)));

        Texture2D destTex = new Texture2D(t1.width, t1.height);
        destTex.SetPixels32(colors);
        destTex.Apply();

        GetComponent<SpriteRenderer>().sprite = Sprite.Create(destTex, new Rect(0, 0, t1.width, t1.height), Vector2.one * 0.5f);

        //---------------------------------------------//

        return score;
    }

    private bool CompareColor32(Color32 c1, Color32 c2, int tolerance)
    {
        if (Mathf.Abs(c1.r - c2.r) > tolerance) return false;
        if (Mathf.Abs(c1.g - c2.g) > tolerance) return false;
        if (Mathf.Abs(c1.b - c2.b) > tolerance) return false;
        if (Mathf.Abs(c1.a - c2.a) > tolerance) return false;

        return true;
    }
}
