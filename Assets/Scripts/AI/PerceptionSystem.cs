using UnityEngine;
using System.Collections.Generic;

public class PerceptionSystem : MonoBehaviour
{
    public string[] detectableTags = { "Coin", "Chest" }; // Tags dos objetos que IA pode perceber

    public List<GameObject> Scan()
    {
        List<GameObject> perceived = new List<GameObject>();

        foreach (string tag in detectableTags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            perceived.AddRange(objects);
        }

        return perceived;
    }

    public List<GameObject> ScanDestructibles()
    {
        List<GameObject> destructibles = new List<GameObject>();
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Destructible");
        destructibles.AddRange(objects);
        return destructibles;
    }

    public bool IsPositionSafe(Vector2 position, List<Vector2> activeBombPositions)
    {
        if (activeBombPositions == null || activeBombPositions.Count == 0)
            return true;

        foreach (Vector2 bombPos in activeBombPositions)
        {
            // Check Manhattan distance (bombs explode in cross pattern)
            int manhattanDist = Mathf.Abs(Mathf.FloorToInt(position.x - bombPos.x)) +
                               Mathf.Abs(Mathf.FloorToInt(position.y - bombPos.y));

            // If within blast radius (Manhattan distance <= 1), not safe
            if (manhattanDist <= 1)
                return false;
        }

        return true;
    }
}
