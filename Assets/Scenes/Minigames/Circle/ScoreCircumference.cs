using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(DrawCircumference))]
public class ScoreCircumference : MonoBehaviour
{
    [SerializeField] private float radius = 2;

    private DrawCircumference drawCircumference;

    private void Awake()
    {
        this.drawCircumference = this.GetComponent<DrawCircumference>();
    }

    private void Start()
    {
        this.drawCircumference.Draw(this.radius);
    }
}
