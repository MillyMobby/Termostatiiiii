using UnityEngine;

public class Cell : MonoBehaviour
{
    public GameObject manager;
    public int objectType;
    public int x, y;
    public Sprite bluePlayer, redPlayer, greenEnemy;
    public bool isButton = false;

    [SerializeField] private SpriteRenderer contentRenderer;
    [SerializeField] private SpriteRenderer borderRenderer;
    // [SerializeField] private SpriteRenderer backgroundRenderer;

    private bool _canAcceptDrop = false;
    private Color originalBorderColor;


    public bool canAcceptDrop
    {
        get
        {
            return this._canAcceptDrop;
        }
        set
        {
            this._canAcceptDrop = value;
        }

    }

    public Sprite getColor(int type)
    {
        // Use the parameter 'type' instead of 'this.objectType'
        // so it actually reflects the NEW data from the server
        switch (type)
        {
            case 1:
                return bluePlayer;
            case 2:
                return redPlayer;
            case 3:
                return greenEnemy;
            default:
                                return null;
        }
    }

    public void UpdateValue(int newValue)
    {
        // Update the internal state
        this.objectType = newValue;

        // Logic: If newValue is 0, maybe the cell is empty? 
        // If not 0, it has an object.
        if (newValue != 0)
        {
            isButton = true;
        }

        // ALWAYS use contentRenderer if it's assigned. 
        // This avoids the "No SpriteRenderer found" error on the parent object.
        if (contentRenderer != null)
        {
            // If the server sends 0, you might want to clear the sprite
            if (newValue == 0)
            {
                contentRenderer.sprite = null;
                isButton = false;
            }
            else
            {
                contentRenderer.sprite = getColor(newValue);
            }
        }
        else
        {
            Debug.LogError($"ContentRenderer is null on {gameObject.name}. Drag the child SpriteRenderer into this slot in the Inspector!");
        }
    }
    public void Init(int x, int y, int objectType)
    {
        this.objectType = objectType;
        this.x = x;
        this.y = y;

        // Set the content sprite
        if (objectType != 0)  // 0 means empty cell
        {
            isButton = true;
            canAcceptDrop = true;

                if (contentRenderer != null)
                {
                    contentRenderer.sprite = getColor(objectType);
                }
        }        
        else
        {
            isButton = false;            
        }
    }

    public void addAsset(SpriteRenderer asset)
    {
        if (contentRenderer != null && asset != null && _canAcceptDrop)
        {
            Debug.Log("changing sprite");
            contentRenderer.sprite = asset.sprite;
            contentRenderer.color = asset.color;
            isButton = true;
            _canAcceptDrop = false;
        }
    }
}