using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Rigidbody2D para controlar a física do player
    Rigidbody2D _rb;

    // Velocidade de movimento horizontal e vertical
    [SerializeField] float xSpeed;
    [SerializeField] float ySpeed;

    // Valores de direção recebidos do input
    float xDir, yDir;

    // Prefab da bomba e taxa de disparo
    [SerializeField] GameObject bombaPrefab;
    [SerializeField] float bombRate;
    float lastBombTime;

    // Vida do player
    [SerializeField] int health = 3;
    public int currentHealth;

    // Invulnerabilidade após receber dano
    [SerializeField] float invulnerabilityTime = 1f;
    float invulnerableTimer = 0f;
    bool isInvulnerable = false;

    // Som de dano
    [SerializeField] AudioClip playerHitSound;
    [SerializeField] AudioClip playerDeathSound;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        lastBombTime = Time.time;
        currentHealth = health;
    }

    void FixedUpdate()
    {
        Movimentar();
    }

    void Update()
    {
        if (isInvulnerable)
        {
            invulnerableTimer -= Time.deltaTime;

            // Piscar o sprite do player para indicar invulnerabilidade
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
    }

    void OnMove(InputValue inputValue)
    {
        xDir = inputValue.Get<Vector2>().x;
        yDir = inputValue.Get<Vector2>().y;
    }

    void OnAttack(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            ColocarBomba();
        }
    }

    void Movimentar()
    {
        _rb.linearVelocityX = xDir * xSpeed * Time.deltaTime;
        _rb.linearVelocityY = yDir * ySpeed * Time.deltaTime;
    }

    void ColocarBomba()
    {
        if (Time.time > lastBombTime + bombRate)
        {
            lastBombTime = Time.time;

            // Centralizar a bomba no grid
            float snappedX = Mathf.Floor(transform.position.x);
            float snappedY = Mathf.Floor(transform.position.y);
            Vector2 snappedPosition = new Vector2(snappedX + 0.5f, snappedY + 0.5f);

            // Instancia o prefab da bomba
            Instantiate(bombaPrefab, snappedPosition, Quaternion.identity);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;

        // Se a vida acabar, reinicia a cena
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

            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            isInvulnerable = true;
            invulnerableTimer = invulnerabilityTime;
            // Som de dano pode ser adicionado aqui
            AudioSource.PlayClipAtPoint(playerHitSound, transform.position);
        }
    }
}
