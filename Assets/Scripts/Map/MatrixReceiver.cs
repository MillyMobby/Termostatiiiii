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
    [SerializeField] private bool logUpdates = true;

    // Matrix data
    private int[] currentMatrix;
    private int simulationStep = 0;
    private float updateTimer = 0f;

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
                    if (currentMatrix[i] == 7)
                    {
                        int row = i / cols;
                        int col = i % cols;
                        Debug.Log($"Frame {simulationStep}: Entity at ({row},{col})");
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

        // Set origin (top-left) - value 1 (red)
        currentMatrix[0] = 1;

        // Set goal (bottom-right) - value 2 (green)
        currentMatrix[(rows - 1) * cols + (cols - 1)] = 2;

        // Set moving entity at DIFFERENT starting position - NOT (0,0)
        // Start at (0,1) instead
        int movingRow = 0;
        int movingCol = 1; // Changed from 0 to 1
        currentMatrix[movingRow * cols + movingCol] = 3;

        if (logUpdates)
        {
            Debug.Log($"Initial matrix generated:");
            Debug.Log($"- Origin at (0,0) = 1 (Red)");
            Debug.Log($"- Goal at ({rows - 1},{cols - 1}) = 2 (Green)");
            Debug.Log($"- Moving entity at ({movingRow},{movingCol}) = 3 (Blue)");
        }
    }

    void UpdateMatrix()
    {
        // Find and clear ONLY the current moving entity (value 3)
        int currentMovingIndex = -1;
        for (int i = 0; i < currentMatrix.Length; i++)
        {
            if (currentMatrix[i] == 3) // Find the blue moving entity
            {
                currentMovingIndex = i;
                break;
            }
        }

        // Clear the old position if found
        if (currentMovingIndex >= 0)
        {
            int row = currentMovingIndex / cols;
            int col = currentMovingIndex % cols;

            // Only clear if it's NOT the goal (rows-1, cols-1)
            // We CAN clear origin (0,0) because the moving entity can pass through it
            if (!(row == rows - 1 && col == cols - 1))
            {
                currentMatrix[currentMovingIndex] = 0; // Set to empty
            }
        }

        simulationStep++;

        // Calculate new position for moving entity
        int movingRow = (simulationStep / cols) % rows;
        int movingCol = simulationStep % cols;
        int newMovingIndex = movingRow * cols + movingCol;

        // Don't overwrite goal (2)
        if (currentMatrix[newMovingIndex] == 2)
        {
            // Skip this position, try next one
            simulationStep++;
            movingRow = (simulationStep / cols) % rows;
            movingCol = simulationStep % cols;
            newMovingIndex = movingRow * cols + movingCol;
        }

        // Set ONLY this cell to 3 (moving entity)
        currentMatrix[newMovingIndex] = 3;

        // DEBUG: Count how many cells have value 3
        int movingCount = 0;
        int originCount = 0;
        int goalCount = 0;

        for (int i = 0; i < currentMatrix.Length; i++)
        {
            if (currentMatrix[i] == 3) movingCount++;
            if (currentMatrix[i] == 1) originCount++;
            if (currentMatrix[i] == 2) goalCount++;
        }

        Debug.Log($"Matrix: Origin={originCount}, Goal={goalCount}, Moving={movingCount} at ({movingRow},{movingCol})");
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
                result += currentMatrix[r * cols + c].ToString().PadLeft(2) + " ";
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
                else if (currentMatrix[i] == 7) movingCount++;
            }

            Debug.Log($"Entities: Origin={originCount}, Goal={goalCount}, Moving={movingCount}");
        }
    }
}