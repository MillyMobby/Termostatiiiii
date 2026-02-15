using UnityEngine;

public class GridAsset : MonoBehaviour
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

    protected SpriteRenderer spriteRenderer;

    public int GridX { get; set; } = -1;
    public int GridY { get; set; } = -1;

    public int currPF;

    #endregion

    #region Grid Settings

    public static int Rows { get; set; }
    public static int Cols { get; set; }

    #endregion

    #region Unity Lifecycle

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateColor();
    }

    #endregion

    #region Grid Placement

    protected bool IsInsideGrid(int row, int col)
    {
        return row >= 0 && row < Rows && col >= 0 && col < Cols;
    }

    protected void ProcessDropOnCell()
    {
        int cellIndex = GridY * Cols + GridX;

        if (cellIndex < 0 || cellIndex >= MapManager.Instance.Cells.Count)
        {
            ResetAsset();
            return;
        }

        Cell targetCell = MapManager.Instance.Cells[cellIndex];

        if (targetCell == null || !targetCell.CanAcceptDrop)
        {
            ResetAsset();
            return;
        }

        int objectType = GetObjectType();

        if (objectType == 0)
        {
            ResetAsset();
            return;
        }

        targetCell.UpdateValue(objectType);
        GridX = targetCell.X;
        GridY = targetCell.Y;

        targetCell.AddAsset(spriteRenderer);

        OnSuccessfullyPlaced();
        UpdateDatabase();
        ResetAsset();
    }

    #endregion

    #region Database Operations

    protected async void UpdateDatabase()
    {
        if (GetObjectType() == 2)
        {
            if (await Requester.ObstacleFound(GridY, GridX))
            {
                Requester.DeleteObstacle(GridY, GridX);
            }
            if (await Requester.MonsterFound(GridY, GridX))
            {
                Requester.DeleteMonster(GridY, GridX);
            }
            Requester.AddMonster(AssignedEntity.Name, AssignedEntity.Current_Pf, GridY, GridX);
            Debug.Log("aggiornamento riuscito");
        }
        else if (GetObjectType() == 1)
        {
            if (await Requester.ObstacleFound(GridY, GridX))
            {
                Requester.DeleteObstacle(GridY, GridX);
            }
            if (await Requester.MonsterFound(GridY, GridX))
            {
                Requester.DeleteMonster(GridY, GridX);
            }
            Requester.AddObstacle(AssignedEntity.Name, AssignedEntity.Current_Pf, GridY, GridX);
            Debug.Log("aggiornamento riuscito");
        }
    }

    #endregion

    #region Helpers

    protected int GetObjectType()
    {
        if (AssignedEntity is Monster) return 2;
        if (AssignedEntity is Character) return 3;
        if (AssignedEntity is Obstacle) return 1;
        return 0;
    }

    protected void UpdateColor()
    {
        if (spriteRenderer == null)
            return;

        if (AssignedEntity is Monster)
            spriteRenderer.color = Color.red;
        else if (AssignedEntity is Character)
            spriteRenderer.color = Color.green;
        else
            spriteRenderer.color = Color.white;
    }

    protected void OnSuccessfullyPlaced()
    {
        Debug.Log($"Placed at ({GridX}, {GridY})");
    }

    protected void ResetAsset()
    {
        Destroy(gameObject);
    }

    public void Initialize(WorldEntity entity, int gridX, int gridY, int pf)
    {
        AssignedEntity = entity;
        GridX = gridX;
        GridY = gridY;
        currPF = pf;

        if (spriteRenderer != null && entity != null)
        {
            // If the entity has a sprite
            if (!string.IsNullOrEmpty("Sprites/Icons/enemyIcon"))
            {
                Sprite entitySprite = Resources.Load<Sprite>("Sprites/Icons/enemyIcon");
                if (entitySprite != null)
                    spriteRenderer.sprite = entitySprite;
            }

            UpdateColor();
        }

    }

    

    #endregion
}

public static class GridAssetFactory
{
    public static T CreateGridAsset<T>(T prefab, WorldEntity entity, int gridX, int gridY, int pf, Transform parent = null) where T : GridAsset
    {
        if (prefab == null)
        {
            Debug.LogError("GridAsset prefab is null!");
            return null;
        }

        T asset = GameObject.Instantiate(prefab, parent);

        asset.Initialize(entity, pf, gridX, gridY);

        return asset;
    }

   
    public static GridAsset CreateGridAsset<T>(WorldEntity entity, int gridX, int gridY, int pf) where T : GridAsset
    {
        GameObject go = new GameObject($"{entity.Name}_GridAsset");
        T asset = go.AddComponent<T>();
        asset.Initialize(entity, pf, gridX, gridY);
        return asset;
    }
}