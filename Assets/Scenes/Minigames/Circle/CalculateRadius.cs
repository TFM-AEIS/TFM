using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CalculateRadius : MonoBehaviour
{
    [SerializeField] private float minRadius = 0.5f;
    [SerializeField] private float maxRadius = 3f;
    [SerializeField] private float difficulty = 0.5f;

    private float time = 0.0f;

    public float Calculate()
    {
        this.time += this.difficulty * Time.deltaTime;
        if (time > 1.0f)
        {
            float temp = this.maxRadius;
            this.maxRadius = this.minRadius;
            this.minRadius = temp;
            this.time = 0.0f;
        }
        return Mathf.Lerp(minRadius, maxRadius, this.time);
    }
}
