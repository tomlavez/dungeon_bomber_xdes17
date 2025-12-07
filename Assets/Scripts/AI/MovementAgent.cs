using UnityEngine;
using System.Collections.Generic;

public class MovementAgent : MonoBehaviour
{
    public float speed = 3f;
    private Queue<Vector2> path = new Queue<Vector2>();
    private Rigidbody2D rb;
    private Collider2D col;

    private float reachThreshold = 0.1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Obtemos o collider do filho
        col = GetComponentInChildren<Collider2D>();
        if (col != null)
        {
            // Verifica se é CircleCollider2D ou outro tipo
            if (col is CircleCollider2D circleCol)
            {
                // Para circle collider, usa o raio diretamente
                float radius = circleCol.radius * Mathf.Max(col.transform.lossyScale.x, col.transform.lossyScale.y);
                reachThreshold = Mathf.Max(radius * 0.8f, 0.15f); // Mínimo de 0.15
                Debug.Log($"[MovementAgent] CircleCollider detectado - Raio: {radius:F3}, Threshold: {reachThreshold:F3}");
            }
            else
            {
                // Para outros colliders (BoxCollider2D, etc), usa o tamanho dos bounds
                float maxSize = Mathf.Max(col.bounds.size.x, col.bounds.size.y);
                reachThreshold = Mathf.Max(maxSize * 0.4f, 0.15f); // Mínimo de 0.15
                Debug.Log($"[MovementAgent] Collider detectado - MaxSize: {maxSize:F3}, Threshold: {reachThreshold:F3}");
            }
        }
        else
        {
            reachThreshold = 0.2f;
            Debug.Log($"[MovementAgent] Nenhum collider encontrado, usando threshold padrão: {reachThreshold}");
        }
    }

    public void SetPath(List<Vector2> newPath)
    {
        path.Clear();
        if (newPath == null) return;

        foreach (var p in newPath)
        {
            path.Enqueue(p); // Não fazemos snap aqui, assumimos que já vem correto
        }

        Debug.Log($"[MovementAgent] Novo caminho definido com {path.Count} waypoints");
    }

    public void Tick()
    {
        if (path.Count == 0)
        {
            return;
        }

        Vector2 targetPos = path.Peek();

        // Usa a posição do Rigidbody (que está no Player pai) como referência
        Vector2 currentPos = rb.position;

        Vector2 delta = targetPos - currentPos;
        float distance = delta.magnitude;

        // Move em direção ao target - usa fixedDeltaTime para movimento com física
        Vector2 move = delta.normalized * Mathf.Min(speed * Time.fixedDeltaTime, distance);
        rb.MovePosition(rb.position + move);

        // Verifica se chegou ao waypoint
        if (distance < reachThreshold)
        {
            Debug.Log($"[MovementAgent] Waypoint alcançado! Distância: {distance:F3}, Threshold: {reachThreshold:F3}");
            path.Dequeue();

            // Snap para a posição exata do waypoint para evitar drift
            rb.MovePosition(targetPos);
        }
    }

    void OnDrawGizmos()
    {
        if (path == null || path.Count == 0) return;

        Gizmos.color = Color.blue;
        Vector2 prev = rb != null ? rb.position : (Vector2)transform.position;

        foreach (var p in path)
        {
            Gizmos.DrawLine(prev, p);
            Gizmos.DrawWireSphere(p, 0.1f);
            prev = p;
        }

        // Posição atual do Rigidbody (Player)
        if (rb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(rb.position, 0.15f);

            // Mostra o threshold de chegada
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rb.position, reachThreshold);
        }

        // Bounds do collider filho
        if (col != null)
        {
            Gizmos.color = Color.green;
            if (col is CircleCollider2D circleCol)
            {
                // Desenha círculo para CircleCollider2D
                Gizmos.DrawWireSphere(col.bounds.center, circleCol.radius);
            }
            else
            {
                // Desenha cubo para outros colliders
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }

        // Mostra o próximo target
        if (path.Count > 0)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(path.Peek(), reachThreshold);
        }
    }
}