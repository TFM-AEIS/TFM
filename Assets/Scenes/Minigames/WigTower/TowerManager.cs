using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private float cameraOffset;

    [SerializeField] private GameObject blockPrefab;
    private MovingBlock currBlock;
    private Stack<MovingBlock> tower;

    [SerializeField] private TextMeshProUGUI scoreText;
    private int score = 0;

    [SerializeField] private float cameraSpeed;
    private float borderLeft, borderRight;
    private float currSpeed = 1;

    void Start()
    {
        this.tower = new Stack<MovingBlock>();

        this.borderLeft = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
        this.borderRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

        this.currBlock = GameObject.Instantiate(blockPrefab, this.transform).GetComponent<MovingBlock>();
        this.currBlock.InitValues(this.GetComponent<Renderer>().bounds.size.x, this.borderLeft, this.borderRight, this.currSpeed, true);
        this.currBlock.transform.position = new Vector3(0, this.GetComponent<Renderer>().bounds.max.y + this.currBlock.BlockExtents.y, 0);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            this.currBlock.Stop();

            float cropAmount = this.currBlock.transform.position.x
                - (this.tower.Count == 0 ? 0 : this.tower.Peek().transform.position.x);
            float newWidth = 2 * this.currBlock.BlockExtents.x - Mathf.Abs(cropAmount);
            
            if (newWidth <= 0)
            {
                this.scoreText.text = "Game Over";
            }
            else
            {
                this.currBlock.Resize(newWidth);
                this.currBlock.transform.position += Vector3.left * 0.5f * cropAmount;
                this.tower.Push(this.currBlock);

                this.score++;
                this.scoreText.text = this.score.ToString();

                this.currBlock = GameObject.Instantiate(this.blockPrefab, this.transform).GetComponent<MovingBlock>();
                this.currBlock.InitValues(newWidth, this.borderLeft, this.borderRight, this.currSpeed, this.tower.Peek().IsMovingLeft);
                this.currBlock.transform.position = this.tower.Peek().transform.position
                    + new Vector3(0, this.tower.Peek().BlockExtents.y + this.currBlock.BlockExtents.y);
            }
        }

        // Movimiento de cámara

        if (this.currBlock.transform.position.y > Camera.main.transform.position.y + cameraOffset)
        {
            Camera.main.transform.Translate(Vector3.up * this.cameraSpeed * Time.deltaTime);
        }
    }
}
