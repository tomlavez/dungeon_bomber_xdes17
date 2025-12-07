using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D _rb;

    [SerializeField] float xSpeed;
    [SerializeField] float ySpeed;

    [SerializeField] GameObject bombaPrefab;
    [SerializeField] float bombRate = 1.5f; // Tempo mínimo entre bombas (em segundos)
    float lastBombTime;

    [SerializeField] int health = 3;
    public int currentHealth;

    [SerializeField] float invulnerabilityTime = 1f;
    float invulnerableTimer = 0f;
    bool isInvulnerable = false;

    [SerializeField] AudioClip playerHitSound;
    [SerializeField] AudioClip playerDeathSound;

    // Novo campo
    [SerializeField] MonoBehaviour controller;
    private ICharacterController _input;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        lastBombTime = -bombRate; // Permite primeira bomba imediatamente
        currentHealth = health;

        // Get all ICharacterController components and find the enabled one
        MonoBehaviour[] controllers = GetComponents<MonoBehaviour>();
        foreach (var controller in controllers)
        {
            if (controller is ICharacterController && controller.enabled)
            {
                _input = controller as ICharacterController;
                break;
            }
        }

        if (_input == null)
        {
            Debug.LogError($"[Player.Awake] No ENABLED ICharacterController found!");
        }
        else
        {
            Debug.Log($"[Player.Awake] Using controller: {(_input as MonoBehaviour).GetType().Name}");
        }
    }

    void FixedUpdate()
    {
        Movimentar();
    }

    void Update()
    {
        if (isInvulnerable)
        {
            AtualizarInvulnerabilidade();
        }

        // Verifica se deve colocar bomba
        if (_input != null)
        {
            bool shouldBomb = _input.ShouldUseBomb();

            if (shouldBomb)
            {
                ColocarBomba();
            }
        }
    }

    void Movimentar()
    {
        if (_input == null) return;

        Vector2 dir = _input.GetMoveDirection();
        _rb.linearVelocityX = dir.x * xSpeed * Time.deltaTime;
        _rb.linearVelocityY = dir.y * ySpeed * Time.deltaTime;
    }

    void ColocarBomba()
    {
        if (bombaPrefab == null)
        {
            Debug.LogError("[Player] BOMB PREFAB IS NULL!");
            return;
        }

        if (Time.time > lastBombTime + bombRate)
        {
            lastBombTime = Time.time;

            float snappedX = Mathf.Floor(transform.position.x);
            float snappedY = Mathf.Floor(transform.position.y);
            Vector2 snappedPosition = new Vector2(snappedX + 0.5f, snappedY + 0.5f);

            GameObject bomb = Instantiate(bombaPrefab, snappedPosition, Quaternion.identity);
        }
    }

    void AtualizarInvulnerabilidade()
    {
        invulnerableTimer -= Time.deltaTime;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = Mathf.Floor(Time.time * 10) % 2 == 0;
        }

        if (invulnerableTimer <= 0f)
        {
            isInvulnerable = false;
            if (sr != null) sr.enabled = true;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            AudioSource.PlayClipAtPoint(playerDeathSound, transform.position);
            print("Game Over");

            GameOverUI gameOverUI = FindFirstObjectByType<GameOverUI>(FindObjectsInactive.Include);
            if (gameOverUI != null)
            {
                gameOverUI.ShowGameOver();
                print("Mostrando Game Over UI");
            }

            gameObject.SetActive(false);
        }
        else
        {
            isInvulnerable = true;
            invulnerableTimer = invulnerabilityTime;
            AudioSource.PlayClipAtPoint(playerHitSound, transform.position);
        }
    }
}
