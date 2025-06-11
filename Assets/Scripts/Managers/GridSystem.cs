using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 2f;
    public Vector3 origin = Vector3.zero; // Settable in Inspector

    private bool[,] occupied;

    private void Awake()
    {
        occupied = new bool[width, height];
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return origin + new Vector3(x * cellSize, 0, y * cellSize);
    }

    public bool IsCellOccupied(int x, int y)
    {
        return occupied[x, y];
    }

    public void SetOccupied(int x, int y, bool value)
    {
        occupied[x, y] = value;
    }

    public bool TryGetGridPosition(Vector3 worldPos, out int x, out int y)
    {
        Vector3 localPos = worldPos - origin;
        x = Mathf.RoundToInt(localPos.x / cellSize);
        y = Mathf.RoundToInt(localPos.z / cellSize);
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    // Optional: Draw grid in Scene view for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = GetWorldPosition(x, y);
                Gizmos.DrawWireCube(pos + new Vector3(cellSize / 2, 0, cellSize / 2), new Vector3(cellSize, 0.1f, cellSize));
            }
        }
    }
}