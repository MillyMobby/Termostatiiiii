using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject manager;
    [SerializeField] private int objectType;
    [SerializeField] private bool isButton = false;

    [Header("Position of the cell")]
    [SerializeField] private int x;
    [SerializeField] private int y;

    [Header("Sprite for entities")]
    [SerializeField] private Sprite redPlayer;
    [SerializeField] private Sprite greenPlayer;
    [SerializeField] private Sprite bluePlayer;


    [Header("Content settings")]
    [SerializeField] private Image cellBackground;
    [SerializeField] private Image contentRenderer;


    public Sprite getColor(int type)
    {
        switch (type)
        {
            case 1: return redPlayer;
            case 2: return greenPlayer;
            case 3: return bluePlayer;
            default: return greenPlayer;
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