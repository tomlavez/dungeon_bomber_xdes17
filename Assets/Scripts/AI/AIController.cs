using UnityEngine;
using System.Collections.Generic;

public class AIController : MonoBehaviour, ICharacterController
{
    public enum AIState
    {
        MovingToTarget,
        ApproachingBombSite,
        PlacingBomb,
        EscapingBlast,
        WaitingForExplosion
    }

    private MovementAgent movementAgent;
    private PerceptionSystem perceptionSystem;
    private DecisionSystem decisionSystem;
    private Pathfinder pathfinder;

    private GameObject lastTarget = null;
    private float lastTargetUtility = 0f; // Score do target atual

    // Bomb strategy fields
    private AIState currentState = AIState.MovingToTarget;
    private List<Vector2> destructiblesToClear = new List<Vector2>();
    private int currentDestructibleIndex = 0;
    private float bombPlacedTime = 0f;
    private Vector2 lastBombPosition;
    private bool shouldUseBombThisFrame = false;
    private float bombPlacementDelayTimer = 0f;
    private const float BOMB_EXPLOSION_TIME = 3.5f;
    private const float BOMB_APPROACH_DISTANCE = 0.15f; // Reduced to ensure AI reaches grid center
    private const float BOMB_PLACEMENT_DELAY = 0.1f; // Wait for movement to fully stop
    private const float SAFE_DISTANCE = 2.0f;

    private void Awake()
    {
        movementAgent = GetComponent<MovementAgent>();
        perceptionSystem = GetComponent<PerceptionSystem>();
        decisionSystem = GetComponent<DecisionSystem>();
        pathfinder = GetComponent<Pathfinder>();
    }

    public Vector2 GetMoveDirection() => Vector2.zero;

    public bool ShouldUseBomb()
    {
        bool result = shouldUseBombThisFrame;
        shouldUseBombThisFrame = false; // Reset after reading
        return result;
    }

    void FixedUpdate()
    {
        // Movimento físico acontece no FixedUpdate
        if (movementAgent != null)
        {
            movementAgent.Tick();
        }
    }

    void Update()
    {
        if (movementAgent == null || perceptionSystem == null ||
            decisionSystem == null || pathfinder == null)
            return;

        Vector2 actualPlayerPos = GetComponent<Rigidbody2D>().position;

        // Handle state machine for bomb strategy
        switch (currentState)
        {
            case AIState.MovingToTarget:
                HandleMovingToTarget(actualPlayerPos);
                break;

            case AIState.ApproachingBombSite:
                HandleApproachingBombSite(actualPlayerPos);
                break;

            case AIState.PlacingBomb:
                HandlePlacingBomb(actualPlayerPos);
                break;

            case AIState.EscapingBlast:
                HandleEscapingBlast(actualPlayerPos);
                break;

            case AIState.WaitingForExplosion:
                HandleWaitingForExplosion(actualPlayerPos);
                break;
        }
    }

    private void HandleMovingToTarget(Vector2 actualPlayerPos)
    {
        // Verifica se tem um target ativo
        if (lastTarget != null)
        {
            // Verifica se o target ainda existe
            if (lastTarget == null)
            {
                lastTarget = null;
                lastTargetUtility = 0f;
                // Continua para escolher novo alvo
            }
            else
            {
                // Target existe, verifica se chegou próximo
                Vector2 lastTargetPos = lastTarget.transform.position;
                float distanceToLastTarget = Vector2.Distance(actualPlayerPos, lastTargetPos);

                if (distanceToLastTarget < 0.3f)
                {
                    movementAgent.SetPath(new List<Vector2>());
                    // Mantém o lastTarget para detectar quando for coletado no próximo frame
                    return;
                }
                else
                {
                    // Ainda está indo para o target, não recalcula
                    return;
                }
            }
        }

        // Se chegou aqui, precisa escolher novo alvo
        var perceived = perceptionSystem.Scan();
        GameObject bestTarget = null;
        float bestUtility = float.MinValue;
        bool useBombStrategy = false;
        Pathfinder.PathResult bestBombPath = null;

        foreach (var obj in perceived)
        {
            if (obj == null) continue;

            Vector2 targetPos = obj.transform.position;
            float distance = Vector2.Distance(actualPlayerPos, targetPos);
            if (distance < 0.1f) distance = 0.1f;

            int value = decisionSystem.GetObjectValue(obj);

            // Try normal path first (only walkable)
            var normalPath = pathfinder.FindPath(actualPlayerPos, targetPos);

            if (normalPath.Count > 0)
            {
                // Normal path exists - always prefer this
                float normalUtility = (value * decisionSystem.valueWeight / normalPath.Count) - (normalPath.Count * decisionSystem.distanceWeight * 0.1f);

                if (normalUtility > bestUtility)
                {
                    bestUtility = normalUtility;
                    bestTarget = obj;
                    useBombStrategy = false;
                    bestBombPath = null;
                }
            }
            else
            {
                // No normal path, try destructible path
                var destructiblePath = pathfinder.FindPathThroughDestructibles(actualPlayerPos, targetPos);

                if (destructiblePath.path.Count > 0 && destructiblePath.totalDestructibles > 0)
                {
                    // Evaluate bomb strategy
                    // Each destructible counts as 3 empty blocks in distance calculation
                    int effectivePathLength = destructiblePath.path.Count + (destructiblePath.totalDestructibles * 2);
                    float destructibleUtility = decisionSystem.EvaluateWithBombStrategy(
                        obj, actualPlayerPos, effectivePathLength, destructiblePath.totalDestructibles);

                    if (destructibleUtility > bestUtility)
                    {
                        bestUtility = destructibleUtility;
                        bestTarget = obj;
                        useBombStrategy = true;
                        bestBombPath = destructiblePath;
                    }
                }
                else
                {
                    Debug.Log($"[AIController] Target at {targetPos}: NO PATH AVAILABLE");
                }
            }
        }

        Debug.Log($"[AIController] BEST TARGET: {(bestTarget != null ? bestTarget.transform.position.ToString() : "NONE")}, useBomb={useBombStrategy}, utility={bestUtility:F2}");

        // Se não encontrou nenhum alvo válido
        if (bestTarget == null)
        {
            movementAgent.SetPath(new List<Vector2>());
            lastTarget = null;
            lastTargetUtility = 0f;
            return;
        }

        // Escolhe o novo alvo
        lastTarget = bestTarget;
        lastTargetUtility = bestUtility;
        Vector2 actualTargetPos = bestTarget.transform.position;

        if (useBombStrategy && bestBombPath != null && bestBombPath.destructiblesToDestroy.Count > 0)
        {
            // Initialize bomb strategy
            Debug.Log($"[AIController] DECISION - Using BOMB strategy to {bestTarget.name} at {actualTargetPos}");
            Debug.Log($"[AIController] DECISION - Destructibles to clear: {bestBombPath.totalDestructibles}, Path waypoints: {bestBombPath.path.Count}");

            destructiblesToClear = new List<Vector2>(bestBombPath.destructiblesToDestroy);

            // Log all destructibles with their health
            Debug.Log($"[AIController] DECISION - Destructibles in path order:");
            for (int i = 0; i < destructiblesToClear.Count; i++)
            {
                Vector2 destPos = destructiblesToClear[i];
                Node destNode = pathfinder.navGrid.WorldToNode(destPos);
                int health = 1;

                if (destNode != null)
                {
                    GameObject destObj = Physics2D.OverlapPoint(destPos)?.gameObject;
                    if (destObj != null)
                    {
                        var destructibleBlock = destObj.GetComponent<DestructibleBlockReinforced>();
                        if (destructibleBlock != null)
                        {
                            health = destructibleBlock.GetHealth();
                        }
                    }
                }

                Debug.Log($"[AIController] DECISION -   [{i}] {destPos} - Health: {health}");
            }

            currentDestructibleIndex = 0;
            currentState = AIState.ApproachingBombSite;

            // Find adjacent walkable position to first destructible
            Vector2 firstDestructible = destructiblesToClear[0];
            Debug.Log($"[AIController] DECISION - First destructible at {firstDestructible}");

            Vector2 adjacentPos = FindAdjacentWalkablePosition(actualPlayerPos, firstDestructible);
            Debug.Log($"[AIController] DECISION - Adjacent position calculated: {adjacentPos}");

            // Navigate to the adjacent position
            var pathToAdjacent = pathfinder.FindPath(actualPlayerPos, adjacentPos);
            if (pathToAdjacent.Count > 0)
            {
                movementAgent.SetPath(pathToAdjacent);
            }
            else
            {
                Debug.LogWarning($"[AIController] No path to adjacent position!");
                movementAgent.SetPath(new List<Vector2> { adjacentPos });
            }
        }
        else
        {
            // Normal path
            Debug.Log($"[AIController] DECISION - Using NORMAL path to {bestTarget.name} at {actualTargetPos}");

            var path = pathfinder.FindPath(actualPlayerPos, actualTargetPos);
            Debug.Log($"[AIController] DECISION - Normal path has {path.Count} waypoints");

            if (path.Count > 0)
            {
                movementAgent.SetPath(path);
            }
            else
            {
                // Try alternative target
                GameObject alternativeTarget = FindAlternativeTarget(perceived, bestTarget, actualPlayerPos);

                if (alternativeTarget != null)
                {
                    Vector2 altTargetPos = alternativeTarget.transform.position;
                    var altPath = pathfinder.FindPath(actualPlayerPos, altTargetPos);

                    if (altPath.Count > 0)
                    {
                        float altDistance = Vector2.Distance(actualPlayerPos, altTargetPos);
                        if (altDistance < 0.1f) altDistance = 0.1f;
                        int altValue = decisionSystem.GetObjectValue(alternativeTarget);

                        lastTarget = alternativeTarget;
                        lastTargetUtility = (altValue * decisionSystem.valueWeight / altDistance);
                        movementAgent.SetPath(altPath);
                    }
                    else
                    {
                        movementAgent.SetPath(new List<Vector2>());
                        lastTarget = null;
                        lastTargetUtility = 0f;
                    }
                }
                else
                {
                    movementAgent.SetPath(new List<Vector2>());
                    lastTarget = null;
                    lastTargetUtility = 0f;
                }
            }
        }
    }

    private void HandleApproachingBombSite(Vector2 actualPlayerPos)
    {
        if (currentDestructibleIndex >= destructiblesToClear.Count)
        {
            // All destructibles cleared, go back to moving to target
            Debug.Log("[AIController] [BOMB STRATEGY] All destructibles cleared, returning to MovingToTarget state");
            currentState = AIState.MovingToTarget;
            return;
        }

        Vector2 destructiblePos = destructiblesToClear[currentDestructibleIndex];
        Vector2 adjacentPos = FindAdjacentWalkablePosition(actualPlayerPos, destructiblePos);
        float distanceToAdjacent = Vector2.Distance(actualPlayerPos, adjacentPos);

        if (distanceToAdjacent < BOMB_APPROACH_DISTANCE)
        {
            // STOP movement IMMEDIATELY before transitioning
            movementAgent.SetPath(new List<Vector2>());
            currentState = AIState.PlacingBomb;
        }
    }

    private void HandlePlacingBomb(Vector2 actualPlayerPos)
    {
        // Wait for movement to fully stop before placing bomb
        if (bombPlacementDelayTimer < BOMB_PLACEMENT_DELAY)
        {
            bombPlacementDelayTimer += Time.deltaTime;
            return;
        }

        // Snap AI position to grid center (where bomb will actually be placed)
        Vector2 snappedBombPos = new Vector2(
            Mathf.Floor(actualPlayerPos.x) + 0.5f,
            Mathf.Floor(actualPlayerPos.y) + 0.5f
        );

        // Signal to place bomb at current snapped position
        shouldUseBombThisFrame = true;
        lastBombPosition = snappedBombPos; // Use snapped current position, not destructible position
        bombPlacedTime = Time.time;
        bombPlacementDelayTimer = 0f; // Reset for next bomb

        currentState = AIState.EscapingBlast;

        // Find safe escape position
        Vector2 safePos = FindSafeEscapePosition(actualPlayerPos, lastBombPosition);

        var escapePath = pathfinder.FindPath(actualPlayerPos, safePos);

        if (escapePath.Count > 0)
        {
            movementAgent.SetPath(escapePath);
        }
        else
        {
            Debug.LogWarning("[AIController] [BOMB STRATEGY] No escape path found!");
            movementAgent.SetPath(new List<Vector2>());
        }
    }

    private void HandleEscapingBlast(Vector2 actualPlayerPos)
    {
        float distanceFromBomb = Vector2.Distance(actualPlayerPos, lastBombPosition);
        float timeElapsed = Time.time - bombPlacedTime;

        // Check if safe distance reached or enough time passed
        if (distanceFromBomb >= SAFE_DISTANCE || timeElapsed > 1.5f)
        {
            Debug.Log($"[AIController] [BOMB STRATEGY] Safe! Waiting for explosion (dist: {distanceFromBomb:F2})");
            currentState = AIState.WaitingForExplosion;
            movementAgent.SetPath(new List<Vector2>());
        }
    }

    private void HandleWaitingForExplosion(Vector2 actualPlayerPos)
    {
        float timeElapsed = Time.time - bombPlacedTime;

        if (timeElapsed < BOMB_EXPLOSION_TIME)
        {
            // Still waiting
            if (Mathf.FloorToInt(timeElapsed) != Mathf.FloorToInt(timeElapsed - Time.deltaTime))
            {
                // Log every second
                Debug.Log($"[AIController] [BOMB STRATEGY] Waiting for explosion... {timeElapsed:F1}s / {BOMB_EXPLOSION_TIME}s");
            }
            return;
        }

        Debug.Log($"[AIController] [BOMB STRATEGY] Explosion time reached ({BOMB_EXPLOSION_TIME}s), checking if destructible cleared");

        // Check if the destructible at current index is destroyed
        Vector2 destructiblePos = destructiblesToClear[currentDestructibleIndex];
        Node node = pathfinder.navGrid.WorldToNode(destructiblePos);

        Debug.Log($"[AIController] [BOMB STRATEGY] Checking node at {destructiblePos}: walkable={node.walkable}, destructible={node.destructible}");

        if (node.walkable)
        {
            // Destructible cleared, move to next
            Debug.Log($"[AIController] [BOMB STRATEGY] SUCCESS! Destructible at {destructiblePos} cleared (node is now walkable)");
            currentDestructibleIndex++;

            if (currentDestructibleIndex < destructiblesToClear.Count)
            {
                // More destructibles to clear
                Debug.Log($"[AIController] [BOMB STRATEGY] Moving to next destructible ({currentDestructibleIndex + 1}/{destructiblesToClear.Count}), transitioning to ApproachingBombSite");
                currentState = AIState.ApproachingBombSite;

                // Find adjacent position to next destructible
                Vector2 nextDestructible = destructiblesToClear[currentDestructibleIndex];
                Vector2 nextAdjacentPos = FindAdjacentWalkablePosition(actualPlayerPos, nextDestructible);

                // Navigate to adjacent position
                var pathToAdjacent = pathfinder.FindPath(actualPlayerPos, nextAdjacentPos);
                if (pathToAdjacent.Count > 0)
                {
                    movementAgent.SetPath(pathToAdjacent);
                }
                else
                {
                    Debug.LogWarning($"[AIController] [BOMB STRATEGY] No path to next position");
                    movementAgent.SetPath(new List<Vector2> { nextAdjacentPos });
                }
            }
            else
            {
                // All destructibles cleared
                Debug.Log("[AIController] [BOMB STRATEGY] All cleared! Going to target");
                currentState = AIState.MovingToTarget;

                if (lastTarget != null)
                {
                    var pathToTarget = pathfinder.FindPath(actualPlayerPos, lastTarget.transform.position);
                    movementAgent.SetPath(pathToTarget);
                }
            }
        }
        else
        {
            // Destructible still there (has more health), place another bomb
            Debug.Log($"[AIController] [BOMB STRATEGY] WARNING: Destructible at {destructiblePos} still blocked (node.walkable={node.walkable})");
            Debug.Log($"[AIController] [BOMB STRATEGY] Block may have more health, transitioning to ApproachingBombSite to place another bomb");
            currentState = AIState.ApproachingBombSite;

            // Already at the destructible position, can approach directly
            movementAgent.SetPath(new List<Vector2>());
        }
    }

    private Vector2 FindSafeEscapePosition(Vector2 currentPos, Vector2 bombPos)
    {
        // Try to find a walkable position at least SAFE_DISTANCE away from bomb
        List<Vector2> candidates = new List<Vector2>();

        // Check positions in a grid pattern around current position
        for (int dx = -3; dx <= 3; dx++)
        {
            for (int dy = -3; dy <= 3; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                Vector2 candidatePos = new Vector2(
                    Mathf.Floor(currentPos.x) + dx + 0.5f,
                    Mathf.Floor(currentPos.y) + dy + 0.5f
                );

                Node node = pathfinder.navGrid.WorldToNode(candidatePos);
                if (node != null && node.walkable)
                {
                    float distFromBomb = Vector2.Distance(candidatePos, bombPos);

                    // Must be outside blast radius (Manhattan distance > 1 from bomb)
                    int manhattanDist = Mathf.Abs(Mathf.FloorToInt(candidatePos.x - bombPos.x)) +
                                       Mathf.Abs(Mathf.FloorToInt(candidatePos.y - bombPos.y));

                    if (manhattanDist > 1 && distFromBomb >= SAFE_DISTANCE)
                    {
                        candidates.Add(candidatePos);
                    }
                }
            }
        }

        // Return closest safe position, or current position if none found
        if (candidates.Count > 0)
        {
            Vector2 closest = candidates[0];
            float closestDist = Vector2.Distance(currentPos, closest);

            foreach (var candidate in candidates)
            {
                float dist = Vector2.Distance(currentPos, candidate);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = candidate;
                }
            }

            Debug.Log($"[AIController] [BOMB STRATEGY] Safe escape position: {closest}");
            return closest;
        }

        // Fallback: move away from bomb
        Debug.LogWarning($"[AIController] [BOMB STRATEGY] No safe positions found! Using fallback");
        Vector2 awayDirection = (currentPos - bombPos).normalized;
        Vector2 fallbackPos = currentPos + awayDirection * SAFE_DISTANCE;
        return fallbackPos;
    }

    /// <summary>
    /// Finds a walkable position adjacent to a destructible block that has a valid path from current position.
    /// Returns the first reachable walkable neighbor (up, down, left, right) to the destructible.
    /// </summary>
    private Vector2 FindAdjacentWalkablePosition(Vector2 fromPosition, Vector2 destructiblePosition)
    {

        // Get the node of the destructible
        Node destructibleNode = pathfinder.navGrid.WorldToNode(destructiblePosition);

        if (destructibleNode == null)
        {
            Debug.LogWarning($"[AIController] [BOMB STRATEGY] Could not find node for destructible at {destructiblePosition}");
            return fromPosition;
        }

        // Check the 4 adjacent cells (up, down, left, right)
        Vector2[] adjacentOffsets = new Vector2[]
        {
            new Vector2(0, -1),  // Up
            new Vector2(0, 1),   // Down
            new Vector2(-1, 0),  // Left
            new Vector2(1, 0)    // Right
        };

        List<Vector2> walkableAdjacent = new List<Vector2>();

        foreach (var offset in adjacentOffsets)
        {
            int adjX = destructibleNode.x + (int)offset.x;
            int adjY = destructibleNode.y + (int)offset.y;

            // Check bounds
            if (adjX >= 0 && adjX < pathfinder.navGrid.width && adjY >= 0 && adjY < pathfinder.navGrid.height)
            {
                Node adjNode = pathfinder.navGrid.grid[adjX, adjY];

                if (adjNode.walkable)
                {
                    Vector2 adjWorldPos = adjNode.worldPos;
                    // Snap to cell center
                    adjWorldPos.x = Mathf.Floor(adjWorldPos.x) + 0.5f;
                    adjWorldPos.y = Mathf.Floor(adjWorldPos.y) + 0.5f;

                    walkableAdjacent.Add(adjWorldPos);
                }
            }
        }

        if (walkableAdjacent.Count == 0)
        {
            Debug.LogWarning($"[AIController] [BOMB STRATEGY] No walkable adjacent positions found! Returning from position");
            return fromPosition;
        }

        // Try to find an adjacent position with a valid path from current position
        Debug.Log($"[AIController] [BOMB STRATEGY] Found {walkableAdjacent.Count} walkable adjacent positions, checking for valid paths...");

        foreach (var adjPos in walkableAdjacent)
        {
            var pathToAdj = pathfinder.FindPath(fromPosition, adjPos);
            if (pathToAdj.Count > 0)
            {
                Debug.Log($"[AIController] [BOMB STRATEGY] Valid path found to adjacent position: {adjPos}");
                return adjPos;
            }
            else
            {
                Debug.Log($"[AIController] [BOMB STRATEGY] No path to adjacent position: {adjPos}, trying next...");
            }
        }

        // If no position has a valid path, return the closest one (fallback)
        Debug.LogWarning($"[AIController] [BOMB STRATEGY] No adjacent position has valid path! Using closest as fallback");
        Vector2 closest = walkableAdjacent[0];
        float closestDist = Vector2.Distance(fromPosition, closest);

        foreach (var pos in walkableAdjacent)
        {
            float dist = Vector2.Distance(fromPosition, pos);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = pos;
            }
        }

        return closest;
    }

    private GameObject FindAlternativeTarget(List<GameObject> perceivedObjects, GameObject excludeTarget, Vector2 playerPos)
    {
        GameObject bestAlternative = null;
        float bestUtility = float.MinValue;

        foreach (var obj in perceivedObjects)
        {
            if (obj == null || obj == excludeTarget) continue;

            float distance = Vector2.Distance(playerPos, obj.transform.position);
            if (distance < 0.1f) distance = 0.1f;

            // Testa se há caminho válido para este objeto
            var testPath = pathfinder.FindPath(playerPos, obj.transform.position);
            if (testPath.Count == 0) continue; // Pula se não houver caminho

            int value = decisionSystem.GetObjectValue(obj);
            float utility = (value / distance);

            if (utility > bestUtility)
            {
                bestUtility = utility;
                bestAlternative = obj;
            }
        }

        return bestAlternative;
    }
}
