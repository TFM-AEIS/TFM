using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Button retryLastButton;
    [SerializeField] private float cameraOffset;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float marginError;

    private MovingBlock currBlock;
    private Stack<MovingBlock> tower;

    private int score = 0;    
    private float borderLeft, borderRight;
    private float currSpeed = 2;

    public int Score 
    { 
        get { return score; } 
        private set 
        {
            this.score = value;
            this.scoreText.text = this.score.ToString();
            this.retryLastButton.interactable = this.score > 0;
        } 
    }

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
        if (Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            this.currBlock.Stop();

            float cropAmount = this.currBlock.transform.position.x
                - (this.tower.Count == 0 ? 0 : this.tower.Peek().transform.position.x);

            float newWidth = 2 * this.currBlock.BlockExtents.x;

            if (Mathf.Abs(cropAmount) > marginError)   // Si el bloque supera el margen de error //
            {
                newWidth -= Mathf.Abs(cropAmount);

                if (newWidth <= 0) // GAME OVER
                {
                    this.feedbackText.text = "GAME OVER";
                    return;
                }

                this.currBlock.Resize(newWidth);
                this.currBlock.transform.position += Vector3.left * 0.5f * cropAmount;
                feedbackText.text = "MISSED!";
                this.currSpeed++;

                StartCoroutine(SliceBlock(cropAmount, 1));
            }
            else    // Si el bloque está dentro del margen de error //
            {
                this.currBlock.transform.position += Vector3.left * cropAmount;
                feedbackText.text = "PERFECT!";
            }

            this.Score++;
            if (this.Score % 10 == 0) this.currSpeed++;

            this.tower.Push(this.currBlock);

            this.currBlock = GameObject.Instantiate(this.blockPrefab, this.transform).GetComponent<MovingBlock>();
            this.currBlock.InitValues(newWidth, this.borderLeft, this.borderRight, this.currSpeed, this.tower.Peek().IsMovingLeft);
            this.currBlock.transform.position = this.tower.Peek().transform.position
                + new Vector3(0, this.tower.Peek().BlockExtents.y + this.currBlock.BlockExtents.y);
        }

        // Movimiento de cámara

        if (this.currBlock.transform.position.y > Camera.main.transform.position.y + cameraOffset)
        {
            Camera.main.transform.Translate(Vector3.up * this.cameraSpeed * Time.deltaTime);
        }
    }

    public void RetryLast()
    {
        this.Score--;
        this.feedbackText.text = "";
        GameObject.Destroy(this.currBlock.gameObject);
        this.currBlock = this.tower.Pop();
        this.currSpeed = this.currBlock.BlockSpeed;
        this.currBlock.InitValues(this.tower.Count == 0 ? this.GetComponent<Renderer>().bounds.size.x : (2 * this.tower.Peek().BlockExtents.x), 
            this.borderLeft, this.borderRight, this.currSpeed, true);
    }

    private IEnumerator SliceBlock(float cropAmount, float waitTime)
    {
        MovingBlock sliceBlock = GameObject.Instantiate(this.blockPrefab, this.transform).GetComponent<MovingBlock>();
        sliceBlock.GetComponent<SpriteRenderer>().color = Color.red;
        sliceBlock.Resize(Mathf.Abs(cropAmount));
        sliceBlock.transform.position = this.currBlock.transform.position 
            + Mathf.Sign(cropAmount) * Vector3.right * (this.currBlock.BlockExtents.x + 0.5f * Mathf.Abs(cropAmount));

        for (float i = 0; i < waitTime; i += Time.deltaTime)
        {
            sliceBlock.transform.Translate(Vector3.down * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        GameObject.Destroy(sliceBlock.gameObject);
    }
}
