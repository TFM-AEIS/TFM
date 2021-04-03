using UnityEngine;

public class Maquillar : MonoBehaviour
{
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Debug.Log("You selected the " + hit.transform.name); // ensure you picked right object

                Texture2D texture = new Texture2D(128, 128);
                hit.transform.GetComponent<Renderer>().material.mainTexture = texture;
                texture.SetPixel(0, 0, Color.black);

                /*for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        Color color = ((x & y) != 0 ? Color.white : Color.gray);
                        Debug.Log("-----");
                        Debug.Log(x);
                        Debug.Log(y);
                        texture.SetPixel(x, y, color);
                    }
                }*/
                texture.Apply();
            }
        }
    }
}
