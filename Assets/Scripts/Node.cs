using UnityEngine;

public class Node
{
    public bool walkable;
    public bool destructible; // Is this node a destructible obstacle?
    public Vector2 worldPos;
    public int x;
    public int y;

    // Custos do A*
    public int gCost;
    public int hCost;
    public Node parent;

    public int fCost => gCost + hCost;

    public Node(bool walkable, Vector2 worldPos, int x, int y, bool destructible = false)
    {
        this.walkable = walkable;
        this.worldPos = worldPos;
        this.x = x;
        this.y = y;
        this.destructible = destructible;
    }
}
