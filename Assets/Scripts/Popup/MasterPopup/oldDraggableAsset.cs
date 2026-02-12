using System.Data;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class OldDraggableAsset : MonoBehaviour
{
    private Character character;
    private Monster monster;
    private Obstacle obstacle; //niente ostacoli e characters per ora, eventually da aggiungere a master scripts
    private Vector3 offset;
    private bool isDragging = false;
    public bool dropped = false;
    private SpriteRenderer spriteRenderer;

    private static int rows;
    public static int Rows
    {
        get => rows;
        set => rows = value;
    }

    private static int cols;
    public static int Cols
    {
        get => cols;
        set => rows = value;
    }

    

    public int gridX = -1, gridY = -1;

    #region PropertiesAndUpdates

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

    private void UpdateColor()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null) return;

        int colorValue = 0;
        if (monster != null)
        {

            colorValue = 2;
        }
        else if (character != null)
        {
            colorValue = 3;
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

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateColor();        
    }

    void Update()
    {
        //if (gridX == -1 && gridY == -1 && dropped) { dropped = false; }
    }

    // Quando clicchi sull'oggetto
    void OnMouseDown()
    {

        if (character != null && character.Assigned)
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
        RectTransform grid = MapManager.Instance.GridCanvas;
        float cellSize = MapManager.Instance.CellSize;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            grid,
            Camera.main.WorldToScreenPoint(transform.position),
            Camera.main,
            out localPoint
        );

        float gridWidth = cols * cellSize;
        float gridHeight = rows * cellSize;

        float startX = - (gridWidth - cellSize) / 2f;
        float startY = - (gridHeight - cellSize) / 2f;

        int col = Mathf.RoundToInt((localPoint.x - startX) / cellSize);
        int row = Mathf.RoundToInt((localPoint.y - startY) / cellSize);

        if (row < 0 || row >= rows || col < 0 || col >= cols)
        {
            ResetDraggable();
            return;
        }

        gridX = col;
        gridY = row;

        float snapX = startX + col * cellSize;
        float snapY = startY + row * cellSize;

        transform.SetParent(grid, true);
        transform.localPosition = new Vector3(snapX, snapY, 0);

        dropped = true;
        ProcessDropOnCell();        
    }


    private void ProcessDropOnCell()
    {
        if (MapManager.Instance == null)
        {
            Debug.LogError("Manager instance is null!");
            return;
        }

        int cellIndex = gridY * cols + gridX;

        if (cellIndex >= 0 && cellIndex < MapManager.Instance.Cells.Count)
        {
            Cell targetCell = MapManager.Instance.Cells[cellIndex];

            if (targetCell != null && targetCell.CanAcceptDrop)
            {
                Debug.Log("Cell can accept drop, processing...");

                SpriteRenderer assetSprite = this.GetComponent<SpriteRenderer>();

                if (assetSprite != null)
                { 
                    if (monster != null)
                    {
                        int objectType = 2; // per ora è static
                        targetCell.UpdateValue(objectType);
                        Debug.Log($"Updated cell with objectType: {objectType}");
                    }
                    else if (character != null)
                    {       
                        int objectType = 3;
                        targetCell.UpdateValue(objectType);
                    }
                    //targetCell.AddAsset(assetSprite);
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
                Debug.Log($"Cell cannot accept drop. TargetCell: {targetCell}, canAcceptDrop: {targetCell?.CanAcceptDrop}");
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

}