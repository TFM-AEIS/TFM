using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    private float pointA, pointB;
    private float blockSpeed;
    private bool isMovingLeft = false;
    private bool isStopped = false;

    private SpriteRenderer rend;

    public Vector3 BlockExtents { get { return this.rend.bounds.extents; } }
    public bool IsMovingLeft { get { return this.isMovingLeft; } }

    //---------------------------------------------------//

    public void InitValues(float width, float a, float b, float speed, bool isMovingLeft)
    {
        this.rend = this.GetComponent<SpriteRenderer>();
        this.Resize(width);

        this.pointA = a + this.BlockExtents.x;
        this.pointB = b - this.BlockExtents.x;
        this.blockSpeed = speed;
        this.isMovingLeft = isMovingLeft;
    }

    public void Resize(float newWidth)
    {
        this.rend.size = new Vector2(newWidth, this.rend.size.y);
    }

    public void Stop() { isStopped = true; }

    //---------------------------------------------------//

    void FixedUpdate()
    {
        if (!isStopped)
        {
            if (isMovingLeft)
            {
                this.transform.Translate(Vector3.left * this.blockSpeed * Time.deltaTime);

                if (this.transform.position.x < this.pointA)
                {
                    this.isMovingLeft = false;
                }
            }
            else
            {
                this.transform.Translate(Vector3.right * this.blockSpeed * Time.deltaTime);

                if (this.transform.position.x > this.pointB)
                {
                    this.isMovingLeft = true;
                }
            }
        }
    }
}
