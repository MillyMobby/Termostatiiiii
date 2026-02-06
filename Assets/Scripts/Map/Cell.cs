using UnityEngine;
using UnityEngine.UI;

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

    private bool canAcceptDrop = false;
    public bool CanAcceptDrop => canAcceptDrop;
    private int objectType;
    public int ObjectType => objectType;
    private bool isButton = false;
    public bool IsButton => isButton;


    public Sprite getColor(int type)
    {
        switch (type)
        {
            case 1: return redPlayer;
            case 2: return greenPlayer;
            case 3: return bluePlayer;
            default: return null;
        }
    }


    public void UpdateValue(int newValue)
    {
        objectType = newValue;

        if (newValue != 0) isButton = true;

        if (contentRenderer != null)
        {
            if (newValue == 0)
            {
                contentRenderer.enabled = false;
                contentRenderer.sprite = null;
                isButton = false;
            } 
            else
            {
                contentRenderer.enabled = true;
                contentRenderer.sprite = getColor(newValue);
                isButton = true;
                canAcceptDrop = true;
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

        GenerateSprite();

        if (objectType != 0)
        {
            isButton = true;
            canAcceptDrop = true;
            
            if (contentRenderer != null) 
            {
                contentRenderer.enabled = true;
                contentRenderer.sprite = getColor(objectType);
            }
        }
        else
        {
            contentRenderer.enabled = false;
            isButton = false;
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
        cellBackground.sprite = Resources.Load<Sprite>(path);
    }

}