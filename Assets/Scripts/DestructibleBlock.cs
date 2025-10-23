using UnityEngine;

public class DestructibleBlockReinforced : MonoBehaviour
{

    [SerializeField] int health;

    [SerializeField] Sprite damagedSprite;
    private SpriteRenderer spriteRenderer;

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
