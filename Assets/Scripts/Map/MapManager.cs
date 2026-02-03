using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private MatrixReceiver _matrixReceiver;
    [SerializeField] private Cell _tilePrefab;
    [SerializeField] private Transform _gridCanvas;
    [SerializeField] private float _cellSize = 100f;

    private int _rows, _cols;
    private int[] inputMatrix;
    private List<Cell> _cells = new List<Cell>();
    private List<DraggableAsset> _masterAssets;


    void Start()
    {
        if (_matrixReceiver != null) 
            _matrixReceiver.OnMatrixReady += HandleNewMapData;
        else 
            Debug.Log("Matrix Receiver has not been assigned in the inspector.");
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
            inputMatrix = new int[_rows * _cols];
            ClearGrid();
            DrawGrid();
            // CenterCamera();
        }

        PrintMat();

        inputMatrix = newMatrix;
        UpdateCellValues();

    }


    void ClearGrid()
    {
        if (_cells == null) return;

        foreach(var cell in _cells)
            if (cell != null) Destroy(cell.gameObject);

        _cells.Clear();
    }


    void DrawGrid()
    {
        if (_rows <= 0 || _cols <= 0) return;
        Vector3 targetScale = new Vector3(_cellSize, _cellSize, 1f);

        float totalWidth = (_cols - 1) * _cellSize;
        float totalHeight = (_rows - 1) * _cellSize;

        float startX = -totalWidth / 2f;
        float startY = -totalHeight / 2f;

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                int index = row * _cols + col;
                if (index >= inputMatrix.Length) break;

                float posX = startX + (col * _cellSize);
                float posY = startY + (row * _cellSize);
                Vector3 spawnPos = new Vector3(posX, posY, 0);

                var spawnedTile = Instantiate(_tilePrefab);
                spawnedTile.transform.GetChild(0).localScale = Vector3.one;
                spawnedTile.transform.SetParent(_gridCanvas, false);
                spawnedTile.transform.localPosition = spawnPos;
                spawnedTile.transform.localScale = targetScale;
                spawnedTile.name = $"Tile ({row}x{col})";
                spawnedTile.Init(row, col, inputMatrix[index]);
                _cells.Add(spawnedTile);
            }
        }
        //_gridCanvas.transform.position = new Vector3(-gridWidth * 0.5f, -gridHeight * 0.5f, 0);
    }


    void UpdateCellValues()
    {
        for (int i = 0; i < inputMatrix.Length; i++)
        {
            if (i < _cells.Count) 
                _cells[i].UpdateValue(inputMatrix[i]);
        }
    }


    private void PrintMat()
    {
        string printable = "[ ";
        foreach (var x in inputMatrix)
            printable += $"{x} ";
        printable += " ]";

        Debug.Log(printable);
    }

}