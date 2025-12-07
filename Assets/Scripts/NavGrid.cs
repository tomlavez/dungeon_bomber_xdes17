using UnityEngine;
using System.Collections.Generic;


public class NavGrid : MonoBehaviour
{

    [Header("Origem do Grid")]
    public Vector2 gridOrigin = Vector2.zero;


    [Header("Configuração da Grade")]
    public int width = 15;
    public int height = 9;
    public float cellSize = 1f;

    [Header("Colisão")]
    public LayerMask obstacleMask;

    public Node[,] grid;

    void Awake()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        grid = new Node[width, height];
        int walkableCount = 0;
        int destructibleCount = 0;
        int blockedCount = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 worldPos = GridToWorld(x, y);
                Collider2D hit = Physics2D.OverlapBox(worldPos, Vector2.one * (cellSize * 0.8f), 0f, obstacleMask);

                bool blocked = hit != null;
                bool isDestructible = false;

                if (blocked && hit.CompareTag("Destructible"))
                {
                    isDestructible = true;
                    destructibleCount++;
                }
                else if (blocked)
                {
                    blockedCount++;
                }
                else
                {
                    walkableCount++;
                }

                grid[x, y] = new Node(!blocked, worldPos, x, y, isDestructible);
            }
        }

        Debug.Log("[NavGrid] Grade gerada.");
    }



    public Vector2 GridToWorld(int x, int y)
    {
        return new Vector2(
            gridOrigin.x + x * cellSize,
            gridOrigin.y - y * cellSize
        );
    }

    public Node WorldToNode(Vector2 worldPosition)
    {
        // Calcula a posição relativa ao grid
        float relX = worldPosition.x - gridOrigin.x;
        float relY = gridOrigin.y - worldPosition.y;

        // Converte para índice da grade
        int x = Mathf.Clamp(Mathf.FloorToInt(relX / cellSize), 0, width - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(relY / cellSize), 0, height - 1);

        return grid[x, y];
    }



    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        int x = node.x;
        int y = node.y;

        if (x > 0) neighbors.Add(grid[x - 1, y]);
        if (x < width - 1) neighbors.Add(grid[x + 1, y]);
        if (y > 0) neighbors.Add(grid[x, y - 1]);
        if (y < height - 1) neighbors.Add(grid[x, y + 1]);

        return neighbors;
    }


    public void UpdateNodeAfterDestruction(Vector2 worldPosition)
    {
        Node node = WorldToNode(worldPosition);
        if (node != null)
        {
            node.walkable = true;
            node.destructible = false;
            Debug.Log($"[NavGrid] Node at ({node.x}, {node.y}) marked as walkable after destruction");
        }
    }

    /// <summary>
    /// Regenera o grid completo. Útil após carregar uma fase do Level Editor.
    /// </summary>
    public void RefreshGrid()
    {
        GenerateGrid();
        Debug.Log("[NavGrid] Grid refreshed");
    }

    void OnDrawGizmos()
    {
        if (grid == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = grid[x, y];
                Vector3 worldPos = GridToWorld(x, y);

                if (node.walkable)
                    Gizmos.color = Color.green;
                else if (node.destructible)
                    Gizmos.color = Color.yellow;
                else
                    Gizmos.color = Color.red;

                Gizmos.DrawCube(worldPos, Vector3.one * (cellSize * 0.9f));
            }
        }
    }



}
