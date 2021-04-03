using UnityEngine;

public class Balanceo : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        this.rb = player.GetComponent<Rigidbody>();
    }

    public void Impulse(float force)
    {
        this.rb.AddTorque(Vector3.forward * force * Time.deltaTime);
    }
}
