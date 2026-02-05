using System.Data;
using UnityEditor.VersionControl;
using UnityEngine;

public class DraggableAsset : MonoBehaviour
{
    private Character character;
    private Monster monster;
    private Obstacle obstacle; //niente ostacoli e characters per ora, eventually da aggiungere a master scripts
    private Vector3 offset;
    private bool isDragging = false;
    public bool dropped = false;
    private SpriteRenderer spriteRenderer;
    private static int rows, cols;

    public int gridX = -1, gridY = -1;

    #region PropertiesAndUpdates
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
    #endregion

    //public static DraggableAsset Create(GameObject prefab, Character c, int color, int pf, Vector3 position)
    //{
    //    // Instantiate the prefab
    //    GameObject obj = Instantiate(prefab, position, Quaternion.identity);

    //    // Get the component
    //    DraggableAsset draggable = obj.GetComponent<DraggableAsset>();
    //    draggable.character = c;

    //    return draggable;
    //}

    ////not uesed
    //public static DraggableAsset Create(GameObject prefab, Monster m, int color, int pf, Vector3 position)
    //{
    //    // Instantiate the prefab
    //    GameObject obj = Instantiate(prefab, position, Quaternion.identity);

    //    // Get the component
    //    DraggableAsset draggable = obj.GetComponent<DraggableAsset>();
    //    draggable.monster = m;

    //    return draggable;
    //}

    ////versions for Obstacle, not used right now
    //public static DraggableAsset Create(GameObject prefab, Obstacle o, int color, int pf, Vector3 position)
    //{
    //    // Instantiate the prefab
    //    GameObject obj = Instantiate(prefab, position, Quaternion.identity);

    //    // Get the component
    //    DraggableAsset draggable = obj.GetComponent<DraggableAsset>();
    //    draggable.obstacle = o;

    //    return draggable;
    //}

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        if (character != null && character.assigned)
            return; // Se già assegnato, non trascinare

        isDragging = true;

        // Calcola l'offset tra mouse e centro dell'oggetto
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = -8;
        offset = transform.position - mouseWorldPos;
        
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = -8;
        transform.position = mouseWorldPos + offset;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;
        SnapToGrid();
        
    }

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
                        int objectType = Monster.color; // per ora è static
                        targetCell.UpdateValue(objectType);
                        Debug.Log($"Updated cell with objectType: {objectType}");
                    }
                    else if (character != null)
                    {
                        int objectType = character.color;
                        targetCell.UpdateValue(objectType);
                    }
                    targetCell.addAsset(assetSprite);
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
        ResetDraggable();
        // The Manager will clean up the null reference in its Update
    }

    private void ResetDraggable()
    {
       Destroy(gameObject);
    }

    // Property per GameEntity
    

    //public void InitializeForDrag(Monster monsterData, Vector3 startPosition)
    //{
    //    this.monster = monsterData;
    //    UpdateColor();

    //    // Position it at the start position
    //    transform.position = startPosition;

    //    // Start dragging immediately
    //    OnMouseDown();

    //    Debug.Log($"Draggable initialized and ready to drag for {monsterData.Name}");
    //}

}