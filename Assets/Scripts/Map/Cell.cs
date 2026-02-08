using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Cell : MonoBehaviour
{
    [SerializeField] private GameObject manager;

    [Header("Position of the cell")]
    [SerializeField] private int x;
    public int X => x;
    [SerializeField] private int y;
    public int Y => y;

    [Header("Sprite for the entities")]
    [SerializeField] private Sprite redPlayer;
    [SerializeField] private Sprite greenPlayer;
    [SerializeField] private Sprite bluePlayer;

    [Header("Content settings")]
    [SerializeField] private Image cellBackground;
    [SerializeField] private Image contentRenderer;

    [Header("Highlight settings")]
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] [Range(0f, 1f)] private float highlightIntensity = 0.3f;
    private Color originalBackgroundColor;
    private bool isHighlighted = false;
    private Coroutine highlightCoroutine;

    private bool canAcceptDrop = false;
    public bool CanAcceptDrop => canAcceptDrop;
    private int objectType;
    public int ObjectType => objectType;
    private bool isButton = false;
    public bool IsButton => isButton;

    void Start()
    {
        // Store the original background color
        if (cellBackground != null)
        {
            originalBackgroundColor = cellBackground.color;
        }
    }


    public Sprite getColor(int type)
    {
        switch (type)
        {
            case 1: return redPlayer;      // Origin = Red
            case 2: return greenPlayer;    // Goal = Green
            case 3: return bluePlayer;     // Moving = Blue
            default: return null;
        }
    }


    public void UpdateValue(int newValue)
    {
        //Debug.Log($"Cell ({x},{y}): Updating from {objectType} to {newValue}");

        // Convert 7 to 3 if needed
        int displayValue = newValue;
        if (newValue == 7) displayValue = 3;

        objectType = displayValue;

        if (newValue != 0)
        {
            isButton = true;
            canAcceptDrop = true;
        }
        else
        {
            isButton = false;
            canAcceptDrop = false;
        }

        if (contentRenderer != null)
        {
            if (newValue == 0)
            {
                contentRenderer.enabled = false;
                contentRenderer.sprite = null;
            }
            else
            {
                contentRenderer.enabled = true;
                contentRenderer.sprite = getColor(displayValue);

                // Debug: Log what sprite we're trying to show
                Sprite sprite = getColor(displayValue);
                if (sprite == null)
                {
                    //Debug.LogError($"Cell ({x},{y}): No sprite for value {displayValue}!");
                }
                else
                {
                    //Debug.Log($"Cell ({x},{y}): Showing {sprite.name} for value {displayValue}");
                }
            }
        }
        else
        {
            Debug.LogError($"ContentRenderer is null on {gameObject.name}!");
        }
    }


    public void Init(int x, int y, int objectType)
    {
        this.x = x;
        this.y = y;

        // Convert 7 to 3 if needed
        int displayValue = objectType;
        if (objectType == 7) displayValue = 3;

        this.objectType = displayValue;

        GenerateSprite();

        if (objectType != 0)
        {
            isButton = true;
            canAcceptDrop = true;

            if (contentRenderer != null)
            {
                contentRenderer.enabled = true;
                contentRenderer.sprite = getColor(displayValue);

                // Debug log
                //Debug.Log($"Cell ({x},{y}): Initialized with value {displayValue}, sprite: {getColor(displayValue)?.name ?? "NULL"}");
            }
        }
        else
        {
            contentRenderer.enabled = false;
            isButton = false;
            canAcceptDrop = false;
        }
    }


    public void AddAsset(SpriteRenderer asset)
    {
        if (contentRenderer != null && asset != null)
        {
            contentRenderer.enabled = true;
            contentRenderer.sprite = asset.sprite;
            contentRenderer.color = asset.color;
            isButton = true;
            canAcceptDrop = false;
        }
    }


    private void GenerateSprite()
    {
        System.Random rnd = new System.Random();
        int value = rnd.Next(1, 5);
        string strVal = value.ToString();

        string path = $"Sprites/Cells/{strVal}";
        Sprite bgSprite = Resources.Load<Sprite>(path);

        if (bgSprite != null && cellBackground != null)
        {
            cellBackground.sprite = bgSprite;
            originalBackgroundColor = cellBackground.color; // Store original color
        }
        else
        {
            Debug.LogWarning($"Cell ({x},{y}): Could not load background sprite from path: {path}");
        }
    }

    /// <summary>
    /// Highlights the cell by modifying the background color
    /// </summary>
    /// <param name="highlight">True to highlight, false to remove highlight</param>
    /// <param name="customColor">Optional custom highlight color (uses default if null)</param>
    public void HighlightCell(bool highlight, Color? customColor = null)
    {
        if (cellBackground == null) return;

        if (highlight)
        {
            Color highlightColorToUse = customColor ?? highlightColor;

            // Tint with highlight color
            Color tintedColor = Color.Lerp(originalBackgroundColor, highlightColorToUse, highlightIntensity);
            cellBackground.color = tintedColor;

            isHighlighted = true;
        }
        else
        {
            // Restore original color
            cellBackground.color = originalBackgroundColor;
            isHighlighted = false;
        }
    }

    /// <summary>
    /// Highlights the cell with red color for 1 second
    /// </summary>
    public void HighlightRedForOneSecond()
    {
        HighlightForOneSecond(Color.red);
    }

    /// <summary>
    /// Highlights the cell with a specific color for 1 second
    /// </summary>
    /// <param name="color">Color to use for highlighting</param>
    public void HighlightForOneSecond(Color color)
    {
        if (cellBackground == null) return;

        // Stop any existing highlight coroutine
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
        }

        // Start new highlight coroutine
        highlightCoroutine = StartCoroutine(HighlightForSecondsCoroutine(color, 1f));
    }

    /// <summary>
    /// Coroutine to highlight the cell for a specific duration
    /// </summary>
    /// <param name="color">Highlight color</param>
    /// <param name="duration">Duration in seconds</param>
    private IEnumerator HighlightForSecondsCoroutine(Color color, float duration)
    {
        if (cellBackground == null) yield break;

        // Store current color before highlighting
        Color currentColor = cellBackground.color;

        // Apply highlight
        Color tintedColor = Color.Lerp(originalBackgroundColor, color, highlightIntensity);
        cellBackground.color = tintedColor;
        isHighlighted = true;

        // Wait for specified duration
        yield return new WaitForSeconds(duration);

        // Restore original color
        cellBackground.color = originalBackgroundColor;
        isHighlighted = false;
        highlightCoroutine = null;
    }

    /// <summary>
    /// Stops any active highlight
    /// </summary>
    public void StopHighlight()
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
            highlightCoroutine = null;
        }

        if (cellBackground != null)
        {
            cellBackground.color = originalBackgroundColor;
            isHighlighted = false;
        }
    }

    // for debugging
    [ContextMenu("Debug This Cell")]
    void DebugCell()
    {
        Debug.Log($"Cell ({x},{y}):");
        Debug.Log($"- ObjectType: {objectType}");
        Debug.Log($"- IsButton: {isButton}");
        Debug.Log($"- CanAcceptDrop: {canAcceptDrop}");
        Debug.Log($"- ContentRenderer enabled: {contentRenderer?.enabled}");
        Debug.Log($"- ContentRenderer sprite: {contentRenderer?.sprite?.name ?? "NULL"}");
        Debug.Log($"- RedPlayer assigned: {redPlayer != null}");
        Debug.Log($"- GreenPlayer assigned: {greenPlayer != null}");
        Debug.Log($"- BluePlayer assigned: {bluePlayer != null}");
        Debug.Log($"- Is highlighted: {isHighlighted}");
        Debug.Log($"- Background color: {cellBackground?.color.ToString() ?? "NULL"}");
    }
}