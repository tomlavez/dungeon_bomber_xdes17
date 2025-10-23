using UnityEngine;

public class Explosao : MonoBehaviour
{

    public int damage = 1;
    public float tempoDeVida = 0.5f; // Meio segundo

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Destructible"))
        {
            DestructibleBlockReinforced block = other.GetComponentInParent<DestructibleBlockReinforced>();
            if (block != null)
            {
                block.TakeDamage(damage);
            }
        }

        if (other.CompareTag("Player"))
        {
            Player player = other.transform.parent.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
