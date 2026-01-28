using UnityEngine;

public class DraggableAsset : MonoBehaviour
{
    private GameEntity gameEntity;
    private Vector3 offset;
    private bool isDragging = false;
    public bool dropped = false;
    private SpriteRenderer spriteRenderer;


// public int x, y;
    public int gridX = -1, gridY = -1;

    // Add this static method to your DraggableAsset class
    public static DraggableAsset Create(GameObject prefab, GameEntity entity, int color, int pf, Vector3 position)
    {
        // Instantiate the prefab
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);

        // Get the component
        DraggableAsset draggable = obj.GetComponent<DraggableAsset>();

        // Initialize it
        draggable.gameEntity = entity;
        entity.color = color;
        entity.current_pf = pf;

        return draggable;
    }

    public void initialize(GameEntity entity, int color, int pf)
    {
        this.gameEntity = entity;
        entity.color = color;
        entity.current_pf = pf; //per provare
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If we already have a gameEntity, update color
        if (gameEntity != null)
        {
            UpdateColor();
        }
    }

    void Update()
    {
        if (gridX == -1 && gridY == -1 && dropped == true) { dropped = false; }
    }

    // Quando clicchi sull'oggetto
    void OnMouseDown()
    {
        if (gameEntity != null && gameEntity.assigned)
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

        // Qui puoi aggiungere uno snap a griglia se vuoi
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

        gridX = Mathf.RoundToInt(cellCoordinates.x);
        gridY = Mathf.RoundToInt(cellCoordinates.y);
        Debug.Log($"dragged su cella {gridX} {gridY}");
        transform.position = new Vector3(0, 7, -5);
        if (gridX != -1 && gridY != -1) { dropped = true; }

    }

    // Property per GameEntity
    public GameEntity Entity
    {
        get { return gameEntity; }
        set
        {
            gameEntity = value;
            UpdateColor(); // Update color when Entity is set
        }
    }

    private void UpdateColor()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null) return;

        switch (gameEntity.color)
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