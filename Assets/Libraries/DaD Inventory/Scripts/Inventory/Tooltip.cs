using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Display tooltip with item info
/// </summary>
public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	public enum TooltipType														// How to display tooltip?
	{
		ShowOnMousePress,														// Display tooltip after mouse clicking (mobile devices)
		ShowOnMouseOver															// Display tooltip on mouse over (PC)
	}
	[Tooltip("On what event tooltip will be shown")]
	public TooltipType tooltipType;
	[Tooltip("Time before tooltip wil be displayed")]
	public float showDelay = 1f;
	[Tooltip("Prefab for tooltip")]
    public GameObject tooltipPrefab;
	[Tooltip("Icon sprite. If null - will be taken from item's image")]
	public Sprite icon;
	[Tooltip("Icon color")]
	public Color color = Color.white;
	[Tooltip("Header field of tooltip")]
	public string header;
    [TextArea(3, 10)]
    public string info;
	[Tooltip("X spacing between elements")]
	public float spacingWidth;
	[Tooltip("Y spacing between elements")]
	public float spacingHeight;


    private static GameObject tooltip;                                          // Current tooltip
    private static float showDelayCounter;                                      // Time counter before tooltip show
	private static Canvas canvas;                                              	// Canvas for Tooltips
	private static string canvasName = "TooltipCanvas";                   		// Name of canvas
	private static int canvasSortOrder = 200;									// Sort order for canvas


	/// <summary>
	/// Raises the disable event.
	/// </summary>
    void OnDisable()
    {
        StopAllCoroutines();                                                    // Stop all coroutines if there is any
        HideTooltip();                                                          // Hide active tooltip
    }

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		if (canvas == null)
		{
			GameObject canvasObj = new GameObject(canvasName);
			canvas = canvasObj.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = canvasSortOrder;
			DontDestroyOnLoad(canvasObj);					
		}
	}

	/// <summary>
	/// Raises the pointer down event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerDown(PointerEventData eventData)
	{
		if (tooltipType == TooltipType.ShowOnMousePress)
		{
			StartCoroutine(DisplayAfterDelay());                               	// Start waiting for cursor idle over object
		}
	}

    /// <summary>
    /// Cursor enter over this object
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
		if (tooltipType == TooltipType.ShowOnMouseOver)
		{
        	StartCoroutine(DisplayAfterDelay());                               	// Start waiting for cursor idle over object
		}
    }

    /// <summary>
    /// Cursor exit this object area
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();                                                    // Stop all coroutines if there is any
        HideTooltip();                                                          // Hide active tooltip
    }

    /// <summary>
    /// Wait for cursor idle and display tooltip
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayAfterDelay()
    {
        showDelayCounter = 0f;
        Vector2 mousePosition = Input.mousePosition;
        while (showDelayCounter < showDelay)                                    // Wait for cursor idle delay
        {
            if ((mousePosition.x == Input.mousePosition.x)
                && (mousePosition.y == Input.mousePosition.y))                  // If cursor in idle state
            {
				showDelayCounter += Time.unscaledDeltaTime;
            }
            else                                                                // If cursor moves
            {
                showDelayCounter = 0f;                                          // Reset delay
                mousePosition = Input.mousePosition;
            }
			yield return new WaitForEndOfFrame();
        }
        DisplayTooltip();                                                       // Show tooltip on display
    }

    /// <summary>
    /// Show tooltip on display
    /// </summary>
    public void DisplayTooltip()
    {
        HideTooltip();                                                          // Hide current tooltip
        if (tooltipPrefab != null)
        {
            tooltip = Instantiate(tooltipPrefab, Input.mousePosition, Quaternion.identity) as GameObject;
            if (canvas != null)
            {
                tooltip.transform.SetParent(canvas.transform, true);            // Set canvas as parent for tooltip
                tooltip.transform.SetAsLastSibling();                           // Put it on top of all GUI
                // Display icon
                if (icon == null)                                               // If no icon specified
                {
                    Image selfImage = null;
                    selfImage = GetComponent<Image>();                          // Try to get image from current object
                    if (selfImage != null)
                    {
                        icon = selfImage.sprite;
                        color = selfImage.color;
                    }
                }
                if (icon != null)
                {
                    Transform iconFolder = tooltip.transform.Find("Icon");      // Get object for icon
                    if (iconFolder != null)
                    {
                        Image image = iconFolder.GetComponent<Image>();
                        if (image != null)
                        {
                            image.sprite = icon;
                            image.color = color;
                            image.raycastTarget = false;
                        }
                    }
                }
                // Display header
                if (header != null)
                {
                    Transform headerFolder = tooltip.transform.Find("Header");  // Get object for header
                    if (headerFolder != null)
                    {
                        Text text = headerFolder.GetComponent<Text>();
                        if (text != null)
                        {
                            text.text = header;
                            text.raycastTarget = false;
                        }
                    }
                }
                // Display info
                if (info != null)
                {
                    Transform infoFolder = tooltip.transform.Find("Info");      // Get object for info text
                    if (infoFolder != null)
                    {
                        Text text = infoFolder.GetComponent<Text>();
                        if (text != null)
                        {
                            text.text = info;
                            text.raycastTarget = false;
                        }
                    }
                }
                RectTransform tooltipRect = tooltip.GetComponent<RectTransform>();
                float totalWidth = 0;
                float totalHeight = 0;
                // Set tooltip size depending on content
                if (tooltipRect != null)
                {
                    Canvas.ForceUpdateCanvases();                               // Render content
                    foreach (Transform child in tooltip.transform)
                    {
                        RectTransform childRect = child.GetComponent<RectTransform>();
                        if (childRect != null)
                        {
                            totalWidth = Mathf.Max(totalWidth, childRect.sizeDelta.x);  // Set tooltip widht
                            totalHeight += childRect.sizeDelta.y;                       // Set tooltip height
                        }
                    }
                    // Update tooltip size
                    tooltipRect.sizeDelta = new Vector2(totalWidth + (spacingWidth * 2),
                                                        totalHeight + (spacingHeight * (tooltip.transform.childCount + 1)));
                    // Shift tooltip depending on screen position
                    float halfWidth = totalWidth / 2;
                    float halfHeight = totalHeight / 2;
                    int halfScreenWidth = Screen.width / 2;
                    int halfScreenHeight = Screen.height / 2;
                    if (Input.mousePosition.x < halfScreenWidth)
                    {
                        if (Input.mousePosition.y > halfScreenHeight)
                        {
                            halfHeight *= -1;                                   // Left upper corner
                        }
                    }
                    else
                    {
                        if (Input.mousePosition.y < halfScreenHeight)
                        {
                            halfWidth *= -1;                                    // Right bottom corner
                        }
                        else
                        {
                            halfWidth *= -1;
                            halfHeight *= -1;                                   // Right upper corner
                        }
                    }
                    // Apply shifting
                    tooltipRect.transform.position = new Vector2(tooltipRect.transform.position.x + halfWidth,
                                                                    tooltipRect.transform.position.y + halfHeight);
                }
            }
        }
    }

    /// <summary>
    /// Remove current tooltip from display
    /// </summary>
    public void HideTooltip()
    {
        if (tooltip != null)
        {
            Destroy(tooltip);
        }
    }
}
