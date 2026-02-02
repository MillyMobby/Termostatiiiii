using System.Data;
using UnityEditor.VersionControl;
using UnityEngine;

public class DraggableAsset : MonoBehaviour
{
    private Character character;
    //private GameEntity gameEntity;
    private Vector3 offset;
    private bool isDragging = false;
    public bool dropped = false;
    private SpriteRenderer spriteRenderer;
    private static int rows, cols;


// public int x, y;
    public int gridX = -1, gridY = -1;

    public static int Rows
    {
        get
        {
            return rows;
        }
        set
        {
            rows = value;
        }
    }
    public static int Cols
    {
        get
        {
            return cols;
        }
        set
        {
            cols = value;
        }
    }
    // Add this static method to your DraggableAsset class
    public static DraggableAsset Create(GameObject prefab, Character c, int color, int pf, Vector3 position)
    {
        // Instantiate the prefab
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);

        // Get the component
        DraggableAsset draggable = obj.GetComponent<DraggableAsset>();

        // Initialize it
        draggable.character = c;
        draggable.character.color = color;
        draggable.character.Name = "Paolo";
        draggable.character.Curr_Pf = pf;
        

        return draggable;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If we already have a gameEntity, update color
        if (character != null)
        {
            UpdateColor();
        }
    }

    void Update()
    {
        //if (gridX == -1 && gridY == -1 && dropped) { dropped = false; }
    }

    // Quando clicchi sull'oggetto
    void OnMouseDown()
    {
        if (character != null && character.assigned)
            return; // Se già assegnato, non trascinare

        isDragging = true;

        // Calcola l'offset tra mouse e centro dell'oggetto
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        offset = transform.position - mouseWorldPos;
        
    }

    // Quando trascini il mouse
    void OnMouseDrag()
    {
        if (!isDragging) return;

        // Segui il mouse
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        transform.position = mouseWorldPos + offset;
    }

    // Quando rilasci il mouse
    void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;


        SnapToGrid();
        
    }

    private void SnapToGrid()
    {

        // for now funziona perchè la griglia viene generata dall'origine, altrimenti si vedrà
        Vector3 cellCoordinates = new Vector3(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y),
            -5
        );

        //transform.position = cellCoordinates;

        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(transform.position.y);
        if (x >= 0 && x < cols && y >= 0 && y < rows) { 
            gridX = x;
            gridY = y;
            Debug.Log($"dragged su cella {gridX} {gridY}");
            transform.position = new Vector3(0, 7, -5);
            if (gridX != -1 && gridY != -1) { dropped = true; }
        }
            

    }

    // Property per GameEntity
    public Character Character
    {
        get { return character; }
        set
        {
            character = value;
            UpdateColor(); // Update color when Entity is set
        }
    }

    private void UpdateColor()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null) return;

        switch (character.color)
        {
            
            case 1: // Blue
                spriteRenderer.color = Color.blue;
                break;
            case 2: // red
                spriteRenderer.color = Color.red;
                break;
            case 3: // green
                spriteRenderer.color = Color.green;
                break;
            default:
                spriteRenderer.color = Color.white;
                break;
        }
    }

}