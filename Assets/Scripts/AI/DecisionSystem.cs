using UnityEngine;
using System.Collections.Generic;

public class DecisionSystem : MonoBehaviour
{
    public float valueWeight = 1.0f;      // Peso do valor
    public float distanceWeight = 1.0f;   // Peso da distância
    public float clusterRadius = 1.5f;    // Raio de cluster

    // Bomb strategy parameters
    public float bombTimeCost = 4.0f;           // Time penalty per bomb in seconds
    public float destructibleDistancePenalty = 2.0f; // Treats each destructible as +N distance

    public GameObject ChooseTarget(List<GameObject> perceivedObjects, Transform self)
    {
        GameObject best = null;
        float bestUtility = float.MinValue;

        Vector2 selfPos = self.position;

        foreach (var obj in perceivedObjects)
        {
            if (obj == null) continue;

            float distance = Vector2.Distance(selfPos, obj.transform.position);
            if (distance < 0.1f) distance = 0.1f;

            int value = GetObjectValue(obj);
            float clusterBonus = CountNearby(pos: obj.transform.position, all: perceivedObjects) * 0.2f;

            // Fórmula melhorada: valor dividido pela distância favorece moedas próximas e valiosas
            float utility = (value * valueWeight / distance) - (distance * distanceWeight * 0.1f) + clusterBonus;

            if (utility > bestUtility)
            {
                bestUtility = utility;
                best = obj;
            }
        }

        return best;
    }

    // Agora suporta Coin e Chest
    public int GetObjectValue(GameObject obj)
    {
        if (obj.TryGetComponent<Coin>(out var coin))
            return coin.GetCoinValue();

        if (obj.TryGetComponent<Chest>(out var chest))
            return chest.GetChestValue();

        return 1; // fallback
    }

    public float EvaluateWithBombStrategy(GameObject target, Vector2 selfPos, int effectivePathLength, int destructibleCount)
    {
        if (target == null || destructibleCount <= 0)
        {
            Debug.Log($"[DecisionSystem] EvaluateWithBombStrategy failed - target null or no destructibles");
            return float.MinValue;
        }

        int value = GetObjectValue(target);

        // Add cluster bonus
        List<GameObject> allTargets = new List<GameObject>();
        allTargets.AddRange(GameObject.FindGameObjectsWithTag("Coin"));
        // Note: Both Coin and Chest use the same "Coin" tag
        float clusterBonus = CountNearby(target.transform.position, allTargets) * 0.2f;

        // Calculate utility with bomb penalty
        // effectivePathLength already includes the 3x penalty for destructibles
        float timePenalty = destructibleCount * bombTimeCost * distanceWeight;

        float utility = (value * valueWeight / effectivePathLength) - timePenalty + clusterBonus;

        return utility;
    }

    private int CountNearby(Vector2 pos, List<GameObject> all)
    {
        int count = 0;

        foreach (var obj in all)
        {
            if (obj == null) continue;
            if (Vector2.Distance(pos, obj.transform.position) <= clusterRadius)
                count++;
        }

        return count;
    }
}
