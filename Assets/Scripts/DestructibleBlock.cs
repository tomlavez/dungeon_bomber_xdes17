using UnityEngine;

public class DestructibleBlockReinforced : MonoBehaviour
{

    [SerializeField] int health;

    [SerializeField] Sprite damagedSprite;
    private SpriteRenderer spriteRenderer;

    // Public getter for health
    public int GetHealth()
    {
        return health;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Notifica o NavGrid que este bloco foi destruído
            NavGrid navGrid = FindFirstObjectByType<NavGrid>();
            if (navGrid != null)
            {
                navGrid.UpdateNodeAfterDestruction(transform.position);
            }

            Destroy(gameObject);
        }
        else if (damagedSprite != null)
        {
            spriteRenderer.sprite = damagedSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
