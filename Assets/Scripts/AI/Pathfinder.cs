using UnityEngine;
using System.Collections.Generic;

public class Pathfinder : MonoBehaviour
{
    public NavGrid navGrid;

    public class PathResult
    {
        public List<Vector2> path;
        public List<Vector2> destructiblesToDestroy;
        public int totalDestructibles;

        public PathResult()
        {
            path = new List<Vector2>();
            destructiblesToDestroy = new List<Vector2>();
            totalDestructibles = 0;
        }
    }

    public List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Node startNode = navGrid.WorldToNode(startPos);
        Node targetNode = navGrid.WorldToNode(targetPos);

        if (!targetNode.walkable)
        {
            return new List<Vector2>();
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        // Inicializa custos do A*
        startNode.gCost = 0;
        startNode.hCost = Heuristic(startNode, targetNode);

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node current = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                Node candidate = openSet[i];

                if (candidate.fCost < current.fCost)
                {
                    current = candidate;
                }
                else if (candidate.fCost == current.fCost)
                {
                    // Em caso de empate, prefere o nó com menor hCost (mais próximo do objetivo)
                    if (candidate.hCost < current.hCost)
                    {
                        current = candidate;
                    }
                    else if (candidate.hCost == current.hCost)
                    {
                        // Tie-breaker: prefere continuar na mesma direção (reduz mudanças de direção)
                        if (current.parent != null && candidate.parent != null)
                        {
                            // Calcula se o candidato mantém a direção do movimento anterior
                            int currentDirX = current.x - current.parent.x;
                            int currentDirY = current.y - current.parent.y;

                            int candidateDirX = candidate.x - candidate.parent.x;
                            int candidateDirY = candidate.y - candidate.parent.y;

                            // Favorece manter a mesma direção
                            bool currentKeepsDirection = (currentDirX == candidateDirX && currentDirY == candidateDirY);
                            bool candidateKeepsDirection = false;

                            if (!currentKeepsDirection)
                            {
                                current = candidate;
                            }
                        }
                        else
                        {
                            // Sem parent, usa distância direta ao target
                            int candidateDist = Mathf.Abs(candidate.x - targetNode.x) + Mathf.Abs(candidate.y - targetNode.y);
                            int currentDist = Mathf.Abs(current.x - targetNode.x) + Mathf.Abs(current.y - targetNode.y);

                            if (candidateDist < currentDist)
                            {
                                current = candidate;
                            }
                        }
                    }
                }
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in navGrid.GetNeighbors(current))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int newCost = current.gCost + 10; // Usa múltiplos de 10 para melhor precisão

                if (newCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCost;
                    neighbor.hCost = Heuristic(neighbor, targetNode);
                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<Vector2>();
    }

    public PathResult FindPathThroughDestructibles(Vector2 startPos, Vector2 targetPos)
    {
        Debug.Log($"[Pathfinder] FindPathThroughDestructibles - Start: {startPos}, Target: {targetPos}");

        PathResult result = new PathResult();

        Node startNode = navGrid.WorldToNode(startPos);
        Node targetNode = navGrid.WorldToNode(targetPos);        // Target must be walkable (coins/chests are always walkable)
        if (!targetNode.walkable)
        {
            return result;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.gCost = 0;
        startNode.hCost = Heuristic(startNode, targetNode);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node current = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                Node candidate = openSet[i];
                if (candidate.fCost < current.fCost ||
                    (candidate.fCost == current.fCost && candidate.hCost < current.hCost))
                {
                    current = candidate;
                }
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == targetNode)
            {
                result.path = RetracePathSimple(startNode, targetNode);

                // Collect destructibles in the path
                Node pathNode = targetNode;
                HashSet<Vector2> destructiblePositions = new HashSet<Vector2>();
                int weightedDestructibleCount = 0;

                while (pathNode != startNode && pathNode != null)
                {
                    if (pathNode.destructible)
                    {
                        Vector2 destructPos = pathNode.worldPos;
                        destructPos.x = Mathf.Floor(destructPos.x) + 0.5f;
                        destructPos.y = Mathf.Floor(destructPos.y) + 0.5f;

                        if (!destructiblePositions.Contains(destructPos))
                        {
                            destructiblePositions.Add(destructPos);
                            result.destructiblesToDestroy.Add(destructPos);

                            // Count weighted destructibles based on health
                            int healthWeight = GetDestructibleHealthWeight(destructPos);
                            weightedDestructibleCount += healthWeight;
                        }
                    }
                    pathNode = pathNode.parent;
                }

                result.destructiblesToDestroy.Reverse(); // Start to end order
                result.totalDestructibles = weightedDestructibleCount;

                Debug.Log($"[Pathfinder] FindPathThroughDestructibles SUCCESS - Path: {result.path.Count} waypoints, Destructibles (physical): {result.destructiblesToDestroy.Count}, Weighted count: {result.totalDestructibles}");

                return result;
            }

            foreach (Node neighbor in navGrid.GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                bool neighborWalkable = neighbor.walkable;
                bool neighborIsDestructible = neighbor.destructible;

                // If not walkable and not destructible, skip (permanent obstacle)
                if (!neighborWalkable && !neighborIsDestructible)
                {
                    continue;
                }

                // Calculate cost: destructibles have high penalty
                int moveCost = neighborIsDestructible ? 100 : 10;
                int newCost = current.gCost + moveCost;

                if (newCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCost;
                    neighbor.hCost = Heuristic(neighbor, targetNode);
                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        Debug.Log("[Pathfinder] FindPathThroughDestructibles FAILED - No path found");
        return result;
    }


    int Heuristic(Node a, Node b)
    {
        // Manhattan distance com escala de 10 para corresponder ao gCost
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y)) * 10;
    }

    List<Vector2> RetracePath(Node start, Node end)
    {
        List<Node> nodes = new List<Node>();
        Node current = end;

        while (current != start)
        {
            nodes.Add(current);
            current = current.parent;
        }

        nodes.Reverse();

        // Aplica path smoothing para remover waypoints desnecessários
        List<Node> smoothedNodes = SmoothPath(nodes);

        List<Vector2> path = new List<Vector2>();
        foreach (var n in smoothedNodes)
        {
            Vector2 pos = n.worldPos;

            // Desloca para o centro da célula
            pos.x = Mathf.Floor(pos.x) + 0.5f;
            pos.y = Mathf.Floor(pos.y) + 0.5f;

            path.Add(pos);
        }

        return path;
    }

    List<Vector2> RetracePathSimple(Node start, Node end)
    {
        List<Node> nodes = new List<Node>();
        Node current = end;

        while (current != start)
        {
            nodes.Add(current);
            current = current.parent;
        }

        nodes.Reverse();

        List<Vector2> path = new List<Vector2>();
        foreach (var n in nodes)
        {
            Vector2 pos = n.worldPos;
            pos.x = Mathf.Floor(pos.x) + 0.5f;
            pos.y = Mathf.Floor(pos.y) + 0.5f;
            path.Add(pos);
        }

        return path;
    }

    List<Node> SmoothPath(List<Node> path)
    {
        if (path.Count <= 2)
            return path;

        List<Node> smoothed = new List<Node>();
        smoothed.Add(path[0]);

        int currentIndex = 0;

        while (currentIndex < path.Count - 1)
        {
            // Tenta pular o máximo de waypoints possível mantendo linha reta
            int farthestIndex = currentIndex + 1;

            for (int i = currentIndex + 2; i < path.Count; i++)
            {
                if (HasLineOfSight(path[currentIndex], path[i]))
                {
                    farthestIndex = i;
                }
                else
                {
                    break;
                }
            }

            smoothed.Add(path[farthestIndex]);
            currentIndex = farthestIndex;
        }

        return smoothed;
    }

    bool HasLineOfSight(Node from, Node to)
    {
        int x0 = from.x;
        int y0 = from.y;
        int x1 = to.x;
        int y1 = to.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;
        int prevX = x0;
        int prevY = y0;

        while (true)
        {
            // Verifica se a célula atual é walkable
            if (x0 >= 0 && x0 < navGrid.width && y0 >= 0 && y0 < navGrid.height)
            {
                if (!navGrid.grid[x0, y0].walkable)
                    return false;
            }
            else
            {
                return false; // Fora do grid
            }

            // Para movimentos diagonais, verifica se os vizinhos adjacentes também são walkable
            // Isso evita "cortar cantos" através de obstáculos
            if (x0 != prevX && y0 != prevY)
            {
                // Movimento diagonal detectado, verifica os dois vizinhos adjacentes
                if (prevX >= 0 && prevX < navGrid.width && y0 >= 0 && y0 < navGrid.height)
                {
                    if (!navGrid.grid[prevX, y0].walkable)
                        return false; // Bloqueado na horizontal
                }

                if (x0 >= 0 && x0 < navGrid.width && prevY >= 0 && prevY < navGrid.height)
                {
                    if (!navGrid.grid[x0, prevY].walkable)
                        return false; // Bloqueado na vertical
                }
            }

            if (x0 == x1 && y0 == y1)
                break;

            prevX = x0;
            prevY = y0;

            int e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the weighted count for a destructible block based on its health.
    /// Health = 1: counts as 1 destructible
    /// Health = 2: counts as 3 destructibles
    /// </summary>
    int GetDestructibleHealthWeight(Vector2 worldPosition)
    {
        // Find the destructible GameObject at this position
        Collider2D hit = Physics2D.OverlapBox(worldPosition, Vector2.one * 0.8f, 0f, navGrid.obstacleMask);

        if (hit != null && hit.CompareTag("Destructible"))
        {
            DestructibleBlockReinforced block = hit.GetComponentInParent<DestructibleBlockReinforced>();
            if (block != null)
            {
                int health = block.GetHealth();

                // Health = 2 counts as 3 destructibles, Health = 1 counts as 1
                int weight = (health == 2) ? 3 : 1;
                Debug.Log($"[Pathfinder] Block at {worldPosition} has health={health}, weight={weight}");
                return weight;
            }
        }

        // Default: count as 1 if we can't determine health
        Debug.Log($"[Pathfinder] Could not determine health for block at {worldPosition}, using weight=1");
        return 1;
    }
}
