using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Manager : MonoBehaviour
{
    // Reference to the receiver script
    [Header("Connections")]
    [SerializeField] private MatrixReceiver _matrixReceiver;

    // These get overwritten by the server data
    private int _rows, _cols;
    private int[] inputMatrix; 

    [SerializeField] private Cell TilePrefab;
    [SerializeField] private Transform _cam;

    private List<Cell> _cells = new List<Cell>();

    private List<DraggableAsset> _masterAssets;
    [SerializeField] private GameObject draggablePrefab;

    void Start()
    {
        Debug.Log($"{_rows}x{_cols}");
        // 1. Subscribe to the event
        if (_matrixReceiver != null)
        {
            _matrixReceiver.OnMatrixReady += HandleNewMapData;
        }
        else
        {
            Debug.LogError("MatrixReceiver is not assigned in the Inspector!");
        }

        // Note: We do NOT call drawGrid() here anymore. 
        // We wait for the server to give us the data first.
    }

    void OnDestroy()
    {
        // Good practice: Unsubscribe when object is destroyed to avoid errors
        if (_matrixReceiver != null)
        {
            _matrixReceiver.OnMatrixReady -= HandleNewMapData;
        }
    }

    // This function runs automatically when MatrixReceiver gets data
// Inside Manager.cs

    void HandleNewMapData(int[] newMatrix, int newRows, int newCols)
    {

        Debug.Log($"BEFORE {_rows}x{_cols}");

        // 1. If the dimensions changed, we must rebuild the grid
        if (newRows != _rows || newCols != _cols)
        {
            _rows = newRows;
            _cols = newCols;
            inputMatrix = new int[_rows * _cols];
            ClearGrid();
            drawGrid();
            CenterCamera();
        }
        
        // 2. Update the visual state of existing cells
        inputMatrix = newMatrix;
        UpdateCellValues();

        Debug.Log($"AFTER {_rows}x{_cols}");

    }

    void UpdateCellValues()
    {
        for (int i = 0; i < inputMatrix.Length; i++)
        {
            if (i < _cells.Count)
            {
                // Update the data inside the Cell without destroying the object
                // You should add a Refresh() method to your Cell class
                _cells[i].UpdateValue(inputMatrix[i]); 
            }
        }
    }

    void ClearGrid()
    {
        foreach (var cell in _cells)
        {
            if(cell != null) Destroy(cell.gameObject);
        }
        _cells.Clear();
    }

    void drawGrid()
    {
        if (_rows == 0 || _cols == 0) return;


        // row-major 
        for (int row = 0; row < _cols; row++)
        {
            for (int col = 0; col < _rows; col++)
            {
                // Safety check to prevent index out of bounds if array is too small
                int index = row * _cols + col;
                if (index >= inputMatrix.Length) break;

                var spawnedTile = Instantiate(TilePrefab, new Vector3(col, row), Quaternion.identity);
                spawnedTile.name = $"Tile {row} {col}";
                
                // Pass the value from the server array
                spawnedTile.Init(row, col, inputMatrix[index]); 
                
                spawnedTile.transform.parent = this.transform;
                _cells.Add(spawnedTile);
            }
        }

        _cam.transform.position = new Vector3((float)_cols / 2 - 0.5f, (float)_rows / 2 - 0.5f, -10);
    }

    void InitializeEntities()
    {
        // Avoid creating duplicates if the map refreshes
        if (_masterAssets != null && _masterAssets.Count > 0) return; 

        GameEntity dummyEntity = new GameEntity();
        DraggableAsset draggable = DraggableAsset.Create(
            draggablePrefab,
            dummyEntity,
            3,  
            20, 
            new Vector3(0, 7, -5) 
        );
        _masterAssets = new List<DraggableAsset> { draggable };
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
                    // Ensure the cell index exists
                    int cellIndex = asset.gridY * _cols + asset.gridX;
                    if(cellIndex < _cells.Count)
                    {
                        SpriteRenderer assetSprite = asset.GetComponent<SpriteRenderer>();
                        _cells[cellIndex].addAsset(assetSprite);
                        asset.gridX = -1;
                        asset.gridY = -1;
                        asset.dropped = false;
                    }
                }
            }
        }
    }

    void HandleTouchInputRaycast()
    {
        Vector2 screenPosition = Vector2.zero;
        bool inputDetected = false;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            screenPosition = Mouse.current.position.ReadValue();
            inputDetected = true;
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            inputDetected = true;
        }

        if (inputDetected)
        {
            Vector2 rayPos = Camera.main.ScreenToWorldPoint(screenPosition);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

            if (hit.collider != null)
            {
                Cell touchedCell = hit.collider.GetComponent<Cell>();
                if (touchedCell != null)
                {
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
        }
    }


    void CenterCamera()
{
    if (_cells == null || _cells.Count == 0) return;

    // 1. Calculate the exact center of the grid 
    // We take the middle of the first and last tile
    Vector3 firstTile = _cells[0].transform.position;
    Vector3 lastTile = _cells[_cells.Count - 1].transform.position;
    
    // The midpoint between the bottom-left and top-right tiles
    Vector3 centerPoint = (firstTile + lastTile) / 2f;

    // 2. Apply to Camera
    _cam.transform.position = new Vector3(centerPoint.x, centerPoint.y, -10f);

    // 3. Auto-Zoom (Orthographic Size)
    // This ensures that no matter the screen size, the grid fills the view
    Camera camComp = _cam.GetComponent<Camera>();
    if (camComp != null)
    {
        float margin = 1.1f; // 10% extra space so tiles aren't touching screen edges
        
        // Vertical fit
        float screenHeightInUnits = _rows / 2f;
        
        // Horizontal fit (accounts for phone vs monitor aspect ratios)
        float screenWidthInUnits = (_cols / 2f) / camComp.aspect;

        camComp.orthographicSize = Mathf.Max(screenHeightInUnits, screenWidthInUnits) * margin;
    }
}
}


