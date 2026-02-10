using UnityEngine;
using System;
using System.Collections.Generic;

public class MatrixReceiver : MonoBehaviour
{
    [Header("Matrix Settings")]
    [SerializeField] private int rows = 6;
    [SerializeField] private int cols = 10;
    [SerializeField] private float updateInterval = 0.5f; // Slower for debugging

    [Header("Debug")]
    [SerializeField] private bool logUpdates = false;

    // Matrix data
    private int[] currentMatrix;
    private int simulationStep = 0;
    private float updateTimer = 0f;

    // Entity positions
    private Vector2Int originPos;
    private Vector2Int goalPos;
    private Vector2Int movingPos;

    // Event for MapManager
    public event Action<int[], int, int> OnMatrixReady;

    void Start()
    {
        Debug.Log("MatrixReceiver: Starting LOCAL matrix simulation");
        Debug.Log($"Matrix size: {rows}x{cols}");
        Debug.Log($"Update rate: {1f / updateInterval:F1} FPS");

        // Initialize matrix
        currentMatrix = new int[rows * cols];
        GenerateMatrix();

        // Send initial matrix immediately
        OnMatrixReady?.Invoke((int[])currentMatrix.Clone(), rows, cols);
    }

    void Update()
    {
        updateTimer += Time.deltaTime;

        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;
            UpdateMatrix();

            // Send updated matrix
            OnMatrixReady?.Invoke((int[])currentMatrix.Clone(), rows, cols);

            if (logUpdates)
            {
                // Find where the moving entity is
                for (int i = 0; i < currentMatrix.Length; i++)
                {
                    if (currentMatrix[i] == 3)
                    {
                        int row = i / cols;
                        int col = i % cols;
                        //Debug.Log($"Frame {simulationStep}: Entity at ({row},{col})");
                        break;
                    }
                }
            }
        }
    }

    void GenerateMatrix()
    {
        // Clear ALL cells to 0 first
        for (int i = 0; i < currentMatrix.Length; i++)
        {
            currentMatrix[i] = 0;
        }

        // Calculate center position (value 1)
        originPos = new Vector2Int(cols / 2, rows / 2);
        currentMatrix[originPos.y * cols + originPos.x] = 1;

        // Set goal (bottom-right) - value 2 (green)
        goalPos = new Vector2Int(cols - 1, rows - 1);
        currentMatrix[goalPos.y * cols + goalPos.x] = 2;

        // Find starting position for moving entity that's not occupied
        movingPos = FindUnoccupiedStartingPosition();
        currentMatrix[movingPos.y * cols + movingPos.x] = 3;

        if (logUpdates)
        {
            Debug.Log($"Initial matrix generated:");
            Debug.Log($"- Origin at ({originPos.x},{originPos.y}) = 1 (Red)");
            Debug.Log($"- Goal at ({goalPos.x},{goalPos.y}) = 2 (Green)");
            Debug.Log($"- Moving entity at ({movingPos.x},{movingPos.y}) = 3 (Blue)");
        }
    }

    Vector2Int FindUnoccupiedStartingPosition()
    {
        // Try different starting positions in order
        List<Vector2Int> possibleStarts = new List<Vector2Int>()
        {
            new Vector2Int(0, 0),           // Top-left
            new Vector2Int(0, rows - 1),    // Bottom-left
            new Vector2Int(cols - 1, 0),    // Top-right
            new Vector2Int(1, 0),           // Near top-left
            new Vector2Int(0, 1),           // Near top-left
        };

        foreach (var pos in possibleStarts)
        {
            int index = pos.y * cols + pos.x;
            if (currentMatrix[index] == 0) // Check if cell is empty
            {
                return pos;
            }
        }

        // Fallback: find any empty cell
        for (int i = 0; i < currentMatrix.Length; i++)
        {
            if (currentMatrix[i] == 0)
            {
                return new Vector2Int(i % cols, i / cols);
            }
        }

        // Last resort: return (0,0)
        return Vector2Int.zero;
    }

    void UpdateMatrix()
    {
        // Clear current moving entity position
        int currentMovingIndex = movingPos.y * cols + movingPos.x;
        currentMatrix[currentMovingIndex] = 0;

        simulationStep++;

        // Calculate next position using a deterministic but varied path
        Vector2Int nextPos = CalculateNextPosition(movingPos, simulationStep);

        // Ensure we don't go out of bounds
        nextPos.x = Mathf.Clamp(nextPos.x, 0, cols - 1);
        nextPos.y = Mathf.Clamp(nextPos.y, 0, rows - 1);

        // If next position is occupied, find nearest unoccupied cell
        int attempts = 0;
        while (IsPositionOccupied(nextPos) && attempts < rows * cols)
        {
            nextPos = FindNearestUnoccupied(nextPos);
            attempts++;
        }

        // Update moving entity position
        movingPos = nextPos;
        currentMatrix[movingPos.y * cols + movingPos.x] = 3;


    }

    Vector2Int CalculateNextPosition(Vector2Int currentPos, int step)
    {
        // Create a varied path pattern using step counter
        int pattern = step % 8;

        switch (pattern)
        {
            case 0: return currentPos + new Vector2Int(1, 0);   // Right
            case 1: return currentPos + new Vector2Int(0, 1);   // Down
            case 2: return currentPos + new Vector2Int(-1, 0);  // Left
            case 3: return currentPos + new Vector2Int(0, -1);  // Up
            case 4: return currentPos + new Vector2Int(1, 1);   // Down-right
            case 5: return currentPos + new Vector2Int(-1, 1);  // Down-left
            case 6: return currentPos + new Vector2Int(1, -1);  // Up-right
            case 7: return currentPos + new Vector2Int(-1, -1); // Up-left
            default: return currentPos + new Vector2Int(1, 0);  // Default to right
        }
    }

    bool IsPositionOccupied(Vector2Int pos)
    {
        int index = pos.y * cols + pos.x;
        return currentMatrix[index] == 1 || currentMatrix[index] == 2;
    }

    Vector2Int FindNearestUnoccupied(Vector2Int startPos)
    {
        // Search in expanding rings around the start position
        for (int radius = 1; radius < Mathf.Max(rows, cols); radius++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    // Only check the outer ring
                    if (Mathf.Abs(dx) == radius || Mathf.Abs(dy) == radius)
                    {
                        Vector2Int testPos = new Vector2Int(
                            Mathf.Clamp(startPos.x + dx, 0, cols - 1),
                            Mathf.Clamp(startPos.y + dy, 0, rows - 1)
                        );

                        if (!IsPositionOccupied(testPos))
                        {
                            return testPos;
                        }
                    }
                }
            }
        }

        // Fallback: return start position (shouldn't happen if there are empty cells)
        return startPos;
    }

    // Public methods for other scripts to access data
    public int[] GetCurrentMatrix(out int outRows, out int outCols)
    {
        outRows = rows;
        outCols = cols;
        return (int[])currentMatrix.Clone();
    }

    public string GetMatrixString()
    {
        if (currentMatrix == null) return "Matrix not initialized";

        string result = $"Matrix {rows}x{cols} (Step {simulationStep}):\n";

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int value = currentMatrix[r * cols + c];
                char symbol = value switch
                {
                    1 => 'O', // Origin
                    2 => 'G', // Goal
                    3 => 'M', // Moving entity
                    _ => '.'  // Empty
                };
                result += symbol + " ";
            }
            result += "\n";
        }

        return result;
    }

    // Debug method to log current matrix state
    [ContextMenu("Log Current Matrix")]
    void LogCurrentMatrix()
    {
        if (currentMatrix != null)
        {
            Debug.Log(GetMatrixString());

            // Count entities
            int originCount = 0, goalCount = 0, movingCount = 0;
            for (int i = 0; i < currentMatrix.Length; i++)
            {
                if (currentMatrix[i] == 1) originCount++;
                else if (currentMatrix[i] == 2) goalCount++;
                else if (currentMatrix[i] == 3) movingCount++;
            }

            Debug.Log($"Entities: Origin={originCount}, Goal={goalCount}, Moving={movingCount}");
            Debug.Log($"Positions: Origin at ({originPos.x},{originPos.y}), Goal at ({goalPos.x},{goalPos.y}), Moving at ({movingPos.x},{movingPos.y})");
        }
    }
}