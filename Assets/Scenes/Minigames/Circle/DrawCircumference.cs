using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawCircumference : MonoBehaviour
{
    private int size;
    private LineRenderer lineDrawer;
    private float thetaScale = 0.01f;
    private float theta = 0f;

    void Awake()
    {
        this.lineDrawer = this.GetComponent<LineRenderer>();
    }

    public void Draw(float radius)
    {
        this.theta = 0f;
        this.size = (int)((1f / this.thetaScale) + 1f);
        this.lineDrawer.positionCount = this.size;
        for (int i = 0; i < this.size; i++)
        {
            this.theta += (2.0f * Mathf.PI * this.thetaScale);
            float x = radius * Mathf.Cos(this.theta);
            float y = radius * Mathf.Sin(this.theta);
            this.lineDrawer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
