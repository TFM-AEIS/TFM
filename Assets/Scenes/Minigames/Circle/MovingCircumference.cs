using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(DrawCircumference))]
[RequireComponent(typeof(CalculateRadius))]
public class MovingCircumference : MonoBehaviour
{
    private DrawCircumference drawCircumference;
    private CalculateRadius calculateRadius;

    private void Awake()
    {
        this.drawCircumference = this.GetComponent<DrawCircumference>();
        this.calculateRadius = this.GetComponent<CalculateRadius>();
    }

    private void Update()
    {
        if (CircleController.playing) this.drawCircumference.Draw(this.calculateRadius.Calculate());
    }
}
