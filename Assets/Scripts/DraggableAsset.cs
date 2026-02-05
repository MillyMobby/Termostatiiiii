using System.Data;
using UnityEditor.VersionControl;
using UnityEngine;

public class DraggableAsset : MonoBehaviour
{
    private Character character;
    private Monster monster;
    private Obstacle obstacle;
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
    public static DraggableAsset Create(GameObject prefab, Character c, int color, int pf, Vector3 position)
    {
        // Instantiate the prefab
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);

        // Get the component
        DraggableAsset draggable = obj.GetComponent<DraggableAsset>();
        draggable.character = c;

        return draggable;
    }

    public static DraggableAsset Create(GameObject prefab, Monster m, int color, int pf, Vector3 position)
    {
        // Instantiate the prefab
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);

        // Get the component
        DraggableAsset draggable = obj.GetComponent<DraggableAsset>();
        draggable.monster = m;

        return draggable;
    }

    public static DraggableAsset Create(GameObject prefab, Obstacle o, int color, int pf, Vector3 position)
    {
        // Instantiate the prefab
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);

        // Get the component
        DraggableAsset draggable = obj.GetComponent<DraggableAsset>();
        draggable.obstacle = o;

        return draggable;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Always update color on start
        UpdateColor();

        Debug.Log($"DraggableAsset started. Monster: {monster != null}, Character: {character != null}");
        
    }

    void Update()
    {
        //if (gridX == -1 && gridY == -1 && dropped) { dropped = false; }
    }

    // Quando clicchi sull'oggetto
    void OnMouseDown()
    {
        // Don't allow dragging until initialized


        if (character != null && character.assigned)
            return; // Se già assegnato, non trascinare

        isDragging = true;

        // Calcola l'offset tra mouse e centro dell'oggetto
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = -8;
        offset = transform.position - mouseWorldPos;
        
    }

    // Quando trascini il mouse
    void OnMouseDrag()
    {
        if (!isDragging) return;

        // Segui il mouse
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = -8;
        transform.position = mouseWorldPos + offset;
    }

    // Quando rilasci il mouse
    void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;


        SnapToGrid();
        
    }

    // In DraggableAsset.cs, update the SnapToGrid() method:
    private void SnapToGrid()
    {
        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(transform.position.y);

        if (x >= 0 && x < cols && y >= 0 && y < rows)
        {
            gridX = x;
            gridY = y;

            // Snap to the grid position
            transform.position = new Vector3(x, y, -8);

            Debug.Log($"dragged su cella {gridX} {gridY}");

            dropped = true;
            Debug.Log($"Asset dropped at ({gridX}, {gridY})");

            // Process the drop immediately
            ProcessDropOnCell();
        }
        else
        {
            // If dropped outside grid, reset
            ResetDraggable();
        }
    }

    private void ProcessDropOnCell()
    {
        if (Manager.Instance == null)
        {
            Debug.LogError("Manager instance is null!");
            return;
        }

        int cellIndex = gridY * cols + gridX;

        // Check if the cell index is valid
        if (cellIndex >= 0 && cellIndex < Manager.Instance.Cells.Count)
        {
            Cell targetCell = Manager.Instance.Cells[cellIndex];

            if (targetCell != null && targetCell.canAcceptDrop)
            {
                Debug.Log("Cell can accept drop, processing...");

                SpriteRenderer assetSprite = this.GetComponent<SpriteRenderer>();

                if (assetSprite != null)
                {  if (monster != null)
                    {
                        int objectType = Monster.color; // Use monster.color not Monster.color
                        targetCell.UpdateValue(objectType);
                        Debug.Log($"Updated cell with objectType: {objectType}");
                    }
                    else if (character != null)
                    {
                        int objectType = character.color;
                        targetCell.UpdateValue(objectType);
                    }
                    // Add the asset to the cell
                    targetCell.addAsset(assetSprite);

                    // Update the cell's objectType based on monster
                    

                    // Successfully placed - we can hide or destroy this draggable
                    OnSuccessfullyPlaced();
                }
                else
                {
                    Debug.LogError("SpriteRenderer is null on draggable!");
                    ResetDraggable();
                }
            }
            else
            {
                Debug.Log($"Cell cannot accept drop. TargetCell: {targetCell}, canAcceptDrop: {targetCell?.canAcceptDrop}");
                ResetDraggable();
            }
        }
        else
        {
            Debug.LogError($"Invalid cell index: {cellIndex}. Grid bounds: {cols}x{rows}");
            ResetDraggable();
        }
    }

    private void OnSuccessfullyPlaced()
    {
        Debug.Log($"Draggable successfully placed at ({gridX}, {gridY})");

        // Option 1: Hide the draggable but keep it in the list
        // gameObject.SetActive(false);
        // transform.position = new Vector3(0, 7, -8);

        // Option 2: Destroy the draggable (cleaner)
        //Destroy(gameObject);

        // The Manager will clean up the null reference in its Update
    }

    public void ResetDraggable()
    {
       Destroy(gameObject);

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

    public Monster Monster
    {
        get { return monster; }
        set
        {
            monster = value;
            UpdateColor(); // Update color when Entity is set
        }
    }

    // In DraggableAsset.cs, update the UpdateColor() method:
    private void UpdateColor()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null) return;

        int colorValue = 0;
        if (monster != null)
        {

            colorValue = Monster.color;
        }
        else if (character != null)
        {
            colorValue = character.color;
        }

        switch (colorValue)
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