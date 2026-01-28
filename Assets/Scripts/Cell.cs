using UnityEngine;

public class Cell : MonoBehaviour
{
    public GameObject manager;
    public int x, y;
    public Sprite bluePlayer, redPlayer, greenEnemy;
    public bool isButton = false;

    [SerializeField] private SpriteRenderer contentRenderer;
    [SerializeField] private SpriteRenderer borderRenderer;
    // [SerializeField] private SpriteRenderer backgroundRenderer;

    private bool canAcceptDrop = false;
    private Color originalBorderColor;



    public void Init(int x, int y, int objectType)
    {
        this.x = x;
        this.y = y;

        // Set the content sprite
        if (objectType != 0)
        {
            isButton = true;
            if (contentRenderer != null)
            {
                switch (objectType)
                {
                    case 1:
                        contentRenderer.sprite = bluePlayer;
                        break;
                    case 2:
                        contentRenderer.sprite = redPlayer;
                        break;
                    case 3:
                        contentRenderer.sprite = greenEnemy;
                        break;
                }
            }
        }
        else
        {
            isButton = false;            
        }
    }

    public void addAsset(SpriteRenderer asset)
    {
        if (contentRenderer != null && asset != null)
        {
            contentRenderer.sprite = asset.sprite;
            contentRenderer.color = asset.color;
            isButton = true;
        }
    }
}