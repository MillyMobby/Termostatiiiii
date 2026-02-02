using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI.Table;

public class Manager : MonoBehaviour

{
    public static Manager Instance { get; private set; }

    //Questa roba poi arriverà dalla connessione col server
    [SerializeField] protected int _rows = 5, _cols = 6;
    [SerializeField] private int[] inputMatrix = { 1, 0, 0, 0, 2, 0, 0, 0, 3, 1, 0, 0, 0, 2, 0, 0, 0, 3, 1, 0, 0, 0, 2, 0, 0, 0, 3, 0 ,0 ,0 };

    [SerializeField] private Cell TilePrefab;
    [SerializeField] private Transform _cam;

    private List<Cell> _cells = new List<Cell> { };

    private List<DraggableAsset> _masterAssets;
    [SerializeField] private GameObject draggablePrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void drawGrid()
    {
        // row-major 
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                int index = row * _cols + col;

                var spawnedTile = Instantiate(TilePrefab, new Vector3(col, row), Quaternion.identity);
                spawnedTile.name = $"Tile {col} {row}";
                spawnedTile.Init(col, row, inputMatrix[index]);
                spawnedTile.transform.parent = this.transform;
                _cells.Add(spawnedTile);
            }
        }
        _cam.transform.position = new Vector3((float)_cols / 2 - 0.5f, (float)_rows / 2 - 0.5f, -10);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //per ora sto creando un draggable a cazzo da qui ma andranno presi dal db o quello che è e istanziati per bene nel menù
        Character dummyEntity = new Character();
        DraggableAsset draggable = DraggableAsset.Create(
            draggablePrefab,
            dummyEntity,
            3,  // green
            20, // pf
            new Vector3(0, 7, -5) 
        );
        _masterAssets = new List<DraggableAsset> { draggable };

        DraggableAsset.Rows = _rows;
        DraggableAsset.Cols = _cols;
        drawGrid();
    }

    // Update is called once per frame
    void Update()
    {
        HandleTouchInputRaycast();
        if (_masterAssets != null)
        {
            foreach (var asset in _masterAssets)
            {
                if (asset.gridX >= 0 && asset.gridX < _cols && asset.gridY >= 0 && asset.gridY < _rows && asset.dropped)
                {
                    if (_cells[asset.gridY * _cols + asset.gridX].canAcceptDrop)
                    {

                        SpriteRenderer assetSprite = asset.GetComponent<SpriteRenderer>();
                        _cells[asset.gridY * _cols + asset.gridX].addAsset(assetSprite);
                        //asset.gridX = -1;
                        //asset.gridY = -1;
                        asset.dropped = false;
                    }
                    else {
                        asset.gridX = -1;
                        asset.gridY = -1;
                    }

                }

            }
        }
    }

    void HandleTouchInputRaycast()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

            if (hit.collider != null)
            {
                Cell touchedCell = hit.collider.GetComponent<Cell>();
                if (touchedCell != null)
                {
                    Debug.Log($"Clicked cell at ({touchedCell.x}, {touchedCell.y})");
                    OnCellTouched(touchedCell.x, touchedCell.y, touchedCell);
                }
            }
        }

        // For touch input on mobile
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Touch touch = Input.touches[0];
            Vector2 rayPos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

            if (hit.collider != null)
            {
                Cell touchedCell = hit.collider.GetComponent<Cell>();
                if (touchedCell != null)
                {
                    Debug.Log($"Touched cell at ({touchedCell.x}, {touchedCell.y})");
                    OnCellTouched(touchedCell.x, touchedCell.y, touchedCell);
                }
            }
        }
    }

    void OnCellTouched(int x, int y, Cell cell)
    {
        if (cell.isButton)
        {
            Debug.Log($"Button cell touched! Coordinates: ({x}, {y})");
            //if (cell.canAcceptDrop == false)
            //{
                for (int i = 0; i < _masterAssets.Count; i++)
                {
                    if (_masterAssets[i].gridX == cell.x && _masterAssets[i].gridY == cell.y)
                    {
                        Debug.Log($"PF = {_masterAssets[i].Character.Curr_Pf}, COLOR = {_masterAssets[i].Character.color}");
                        return;

                    }


                }
                // qui andranno mostrate le info della entity cliccata
            
        }

    }

    

}
