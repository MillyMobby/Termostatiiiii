using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{

    // * Singleton
    public static MapManager Instance { get; private set; }

    [Header("Map Settings")]
    [SerializeField] private MatrixReceiver _matrixReceiver;
    [SerializeField] private Cell _tilePrefab;
    [SerializeField] private RectTransform _gridCanvas;
    public RectTransform GridCanvas => _gridCanvas;

    [SerializeField] private float _cellSize = 100f;
    public float CellSize => _cellSize;

    [Header("Draggable settings")]
    [SerializeField] private GameObject draggablePrefab;
    private List<DraggableAsset> _masterAssets;
    private Dictionary<Character, Vector2Int> players = new Dictionary<Character, Vector2Int>();
    public Dictionary<Character, Vector2Int> Players
    {
        get => players;
        set => players = value;
    }


    [Header("Data")]
    private int _rows, _cols;
    private int[] inputMatrix;
    private List<Cell> cells = new List<Cell> { };
    public List<Cell> Cells => cells;

    public void AssignCharacterInGrid(int color, Character player)
    {
        for (int i = 0; i < inputMatrix.Length; i++)
        {
            if (inputMatrix[i] == color)
            {
                int row = i / _cols;
                int col = i % _cols;

                players[player] = new Vector2Int(col, row);

            }
        }
    }


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    void Start()
    {
        if (_matrixReceiver != null) 
            _matrixReceiver.OnMatrixReady += HandleNewMapData;
        else 
            Debug.Log("Matrix Receiver has not been assigned in the inspector.");
        
        _masterAssets = new List<DraggableAsset>();
        DraggableAsset.Rows = _rows;
        DraggableAsset.Cols = _cols;
    }


    void Update()
    {
        HandleTouchInputRaycast();
    }


    void OnDestroy()
    {
        if (_matrixReceiver != null) 
            _matrixReceiver.OnMatrixReady -= HandleNewMapData;
    }


    void HandleNewMapData(int[] newMatrix, int newRows, int newCols)
    {
        if (newRows != _rows || newCols != _cols)
        {
            _rows = newRows;
            _cols = newCols;

            DraggableAsset.Rows = _rows;
            DraggableAsset.Cols = _cols;
            inputMatrix = new int[_rows * _cols];

            ClearGrid();
            DrawGrid();
        } 
        UpdateGrid(newMatrix);
    }


    void ClearGrid()
    {
        if (cells == null) return;

        foreach(var cell in cells)
            if (cell != null) Destroy(cell.gameObject);

        cells.Clear();
    }


    void DrawGrid()
    {
        if (_rows <= 0 || _cols <= 0) return;

        // 1. Calculate and set the Canvas size
        float gridWidth = _cols * _cellSize;
        float gridHeight = _rows * _cellSize;
        _gridCanvas.sizeDelta = new Vector2(gridWidth, gridHeight);

        Vector3 targetScale = new Vector3(_cellSize, _cellSize, 1f);

        // 2. Calculate the start position so the grid is centered within the canvas
        // We use (cols - 1) for the offset because tile pivots are usually at their center
        float startX = -(gridWidth - _cellSize) / 2f;
        float startY = -(gridHeight - _cellSize) / 2f;

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                int index = row * _cols + col;
                if (index >= inputMatrix.Length) break;

                float posX = startX + (col * _cellSize);
                float posY = startY + (row * _cellSize);
                
                var spawnedTile = Instantiate(_tilePrefab);
                spawnedTile.transform.SetParent(_gridCanvas, false);
                
                // Using localPosition because it's relative to the _gridCanvas center
                spawnedTile.transform.localPosition = new Vector3(posX, posY, 0);
                spawnedTile.transform.localScale = targetScale;
                
                spawnedTile.name = $"Tile ({row}x{col})";
                spawnedTile.Init(row, col, inputMatrix[index]);
                cells.Add(spawnedTile);
            }
        }
    }
    public void UpdateGrid(int[] newMatrix)
    {
        List<int> changedIndices = new List<int>();
        List<int> values = new List<int>();
        List<Sprite> sprites = new List<Sprite>();

        for (int i = 0; i < inputMatrix.Length; i++)
        {
            if (inputMatrix[i] != newMatrix[i]) 
            {
                changedIndices.Add(i);
                values.Add(inputMatrix[i]);
                Debug.Log($"Something has changed at index {i}, old value {inputMatrix[i]} - new value {newMatrix[i]}");
            }
        }

        if (changedIndices.Count > 2)
        {
            inputMatrix = newMatrix;
            ClearGrid();
            DrawGrid();
            return;
        }

        if (changedIndices.Count == 2)
        {
            SwapCells(changedIndices[0], changedIndices[1], values[0], values[1]);
        }

        inputMatrix = (int[])newMatrix.Clone();
    }

    private void SwapCells(int indexA, int indexB, int objectTypeA, int objectTypeB)
    {
        if (indexA >= cells.Count || indexB >= cells.Count) return;

        Cell cellA = cells[indexA];
        Cell cellB = cells[indexB];

        Sprite spriteA = cellA.ContentRenderer.sprite;
        Sprite spriteB = cellB.ContentRenderer.sprite;

        Sprite tempSprite = cellA.CellBackground.sprite;
        cellA.CellBackground.sprite = cellB.CellBackground.sprite;
        cellB.CellBackground.sprite = tempSprite;

        Vector3 tempPos = cellA.transform.localPosition;
        cellA.transform.localPosition = cellB.transform.localPosition;
        cellB.transform.localPosition = tempPos;

        int tempX = cellA.X;
        int tempY = cellA.Y;

        cellA.UpdateCoordinates(cellB.X, cellB.Y);
        cellB.UpdateCoordinates(tempX, tempY);

        cellA.Init(cellB.X, cellB.Y, objectTypeA, generateBackground: false, sprite: spriteA); 
        cellB.Init(tempX, tempY, objectTypeB, generateBackground: false, sprite: spriteB);
    
        cells[indexA] = cellB;
        cells[indexB] = cellA;
    }


    public void AddDraggableAsset(DraggableAsset asset)
    {
        if(_masterAssets == null)
            _masterAssets = new List<DraggableAsset>();

        _masterAssets.Add(asset);
    }


    void HandleTouchInputRaycast()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

            if (hit.collider != null)
            {
                Cell touchedCell = hit.collider.GetComponentInParent<Cell>();
                if (touchedCell != null)
                {
                    touchedCell.HighlightForOneSecond(Color.green);
                    Debug.Log($"Clicked cell at ({touchedCell.X}, {touchedCell.Y})");
                    OnCellTouched(touchedCell.X, touchedCell.Y, touchedCell);
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
                Cell touchedCell = hit.collider.GetComponentInParent<Cell>();
                if (touchedCell != null)
                {
                    touchedCell.HighlightForOneSecond(Color.green);
                    Debug.Log($"Touched cell at ({touchedCell.X}, {touchedCell.Y})");
                    OnCellTouched(touchedCell.X, touchedCell.Y, touchedCell);
                }
            }
        }
    }


    void OnCellTouched(int x, int y, Cell cell)
    {
        if (cell.IsButton)
        {
            Debug.Log($"Button cell touched! Coordinates: ({x}, {y})");

            for (int i = 0; i < _masterAssets.Count; i++)
            {
                if (_masterAssets[i].GridX == cell.X && _masterAssets[i].GridY == cell.Y)
                {
                    Debug.Log($"PF = {_masterAssets[i].AssignedEntity.Max_Pf}, AC = {_masterAssets[i].AssignedEntity.Bio}");
                    return;

                }


            }
            // ! qui andranno mostrate le info della entity cliccata
        }

    }
    public void HighlightArea(int color, int range)
    {
        for (int i = 0; i < inputMatrix.Length; i++)
        {
            if (inputMatrix[i] == color)
            {
                int row = i / _cols;
                int col = i % _cols;

                //  bounds
                int startX = col - range;
                int endX = col + range;
                int startY = row - range;
                int endY = row + range;

                startX = Mathf.Max(startX, 0);
                endX = Mathf.Min(endX, _cols - 1);
                startY = Mathf.Max(startY, 0);
                endY = Mathf.Min(endY, _rows - 1);

                // Highlight the square area
                for (int y = startY; y <= endY; y++)
                {
                    for (int x = startX; x <= endX; x++)
                    {
                        int index = y * _cols + x;
                        if (index >= 0 && index < cells.Count)
                        {
                            Cell c = cells[index];
                            if (c != null)
                            {
                                c.HighlightForOneSecond(Color.red);
                            }
                        }
                    }
                }
            }
        }
    }

}