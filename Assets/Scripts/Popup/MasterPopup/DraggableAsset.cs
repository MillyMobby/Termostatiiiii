using UnityEngine;

public class DraggableAsset : MonoBehaviour
{
    #region Fields

    [Header("Assigned Entities")]

    private WorldEntity assignedEntity;
    public WorldEntity AssignedEntity
    {
        get => assignedEntity;
        set
        {
            assignedEntity = value;
            UpdateColor();
        }
    }

    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    private Vector3 dragOffset;
    private bool isDragging;

    public bool Dropped { get; private set; }

    public int GridX { get; private set; } = -1;
    public int GridY { get; private set; } = -1;

    #endregion

    #region Grid Settings

    public static int Rows { get; set; }
    public static int Cols { get; set; }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        UpdateColor();
    }

    #endregion

    #region Drag Logic

    private void OnMouseDown()
    {
        if (AssignedEntity is Character c && c.Assigned)
            return;

        isDragging = true;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        dragOffset = transform.position - mouseWorldPos;
    }

    private void OnMouseDrag()
    {
        if (!isDragging)
            return;

        transform.position = GetMouseWorldPosition() + dragOffset;
    }

    private void OnMouseUp()
    {
        if (!isDragging)
            return;

        isDragging = false;
        SnapToGrid();
    }

    #endregion

    #region Grid Placement

    private void SnapToGrid()
    {
        RectTransform grid = MapManager.Instance.GridCanvas;
        float cellSize = MapManager.Instance.CellSize;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            grid,
            mainCamera.WorldToScreenPoint(transform.position),
            mainCamera,
            out Vector2 localPoint))
        {
            ResetDraggable();
            return;
        }

        float gridWidth = Cols * cellSize;
        float gridHeight = Rows * cellSize;

        float startX = -(gridWidth - cellSize) / 2f;
        float startY = -(gridHeight - cellSize) / 2f;

        int col = Mathf.RoundToInt((localPoint.x - startX) / cellSize);
        int row = Mathf.RoundToInt((localPoint.y - startY) / cellSize);

        if (!IsInsideGrid(row, col))
        {
            ResetDraggable();
            return;
        }

        GridX = col;
        GridY = row;

        transform.SetParent(grid, true);
        transform.localPosition = new Vector3(
            startX + col * cellSize,
            startY + row * cellSize,
            0f
        );

        Dropped = true;
        ProcessDropOnCell();
    }

    private bool IsInsideGrid(int row, int col)
    {
        return row >= 0 && row < Rows && col >= 0 && col < Cols;
    }

    private async void ProcessDropOnCell()
    {
        int cellIndex = GridY * Cols + GridX;

        if (cellIndex < 0 || cellIndex >= MapManager.Instance.Cells.Count)
        {
            ResetDraggable();
            return;
        }

        Cell targetCell = MapManager.Instance.Cells[cellIndex];

        if (targetCell == null || !targetCell.CanAcceptDrop)
        {
            ResetDraggable();
            return;
        }

        int objectType = GetObjectType();
        if (objectType == 0)
        {
            ResetDraggable();
            return;
        }

        targetCell.UpdateValue(objectType);
        if (GetObjectType() == 2)
        {
            
            Debug.Log($"Sono un mostro e mi trovo in X = {GridX}; Y = {GridY}");
            if (await Requester.ObstacleFound(GridX, GridY))
            {
                Debug.Log("Cancello ostacolo precedente");
                Requester.DeleteObstacle(GridX, GridY);
            }
            if (await Requester.MonsterFound(GridX, GridY))
            {
                Debug.Log("Cancello mostro precedente");
                Requester.DeleteMonster(GridX, GridY);
            }
            Requester.AddMonster(AssignedEntity.Name, AssignedEntity.Current_Pf, GridX, GridY);
        }
        else if (GetObjectType() == 1)
        {
            Debug.Log($"Sono un ostacolo e mi trovo in X = {GridX}; Y = {GridY}");
            if (await Requester.ObstacleFound(GridX, GridY))
            {
                Debug.Log("Cancello ostacolo precedente");
                Requester.DeleteObstacle(GridX, GridY);
            }
            if (await Requester.MonsterFound(GridX, GridY))
            {
                Debug.Log("Cancello mostro precedente");
                Requester.DeleteMonster(GridX, GridY);
            }
            Requester.AddObstacle(AssignedEntity.Name, AssignedEntity.Current_Pf, GridX, GridY);
        }
        targetCell.AddAsset(spriteRenderer, AssignedEntity);

        OnSuccessfullyPlaced();
    }

    #endregion

    #region Helpers

    private int GetObjectType()
    {
        if (AssignedEntity is Monster m) return 2;
        if (AssignedEntity is Character c) return 3;
        if (AssignedEntity is Obstacle o) return 1;
        return 0;
    }

    private void UpdateColor()
    {
        if (spriteRenderer == null)
            return;

        if (AssignedEntity is Monster m) 
            spriteRenderer.color = Color.red;
        else if (AssignedEntity is Character c)
            spriteRenderer.color = Color.green;
        else
            spriteRenderer.color = Color.white;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = -8f;
        return pos;
    }

    private void OnSuccessfullyPlaced()
    {
        //Debug.Log($"Placed at ({GridX}, {GridY})");
        ResetDraggable();
    }

    private void ResetDraggable()
    {
        Destroy(gameObject);
    }

    #endregion
}
