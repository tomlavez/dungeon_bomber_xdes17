using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel; // seu painel criado
    [SerializeField] TextMeshProUGUI scoreText; // opcional, para mostrar a pontuação

    void Start()
    {
        // Garante que o painel comece desativado
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver(int score = 0)
    {
        // Desabilitar Level Editor se estiver ativo
        LevelEditor levelEditor = FindFirstObjectByType<LevelEditor>();
        if (levelEditor != null && levelEditor.IsEditModeActive())
        {
            levelEditor.DisableEditMode();
        }

        // Pausa o jogo
        Time.timeScale = 0f;

        // Ativa o painel
        gameOverPanel.SetActive(true);

        // Atualiza o score, se tiver
        if (scoreText != null && ScoreManager.instance != null)
        {
            scoreText.text = "Score: " + ScoreManager.instance.score;
        }
    }

    public void PlayAgain()
    {
        // Verifica se há um Level Editor na cena
        LevelEditor levelEditor = FindFirstObjectByType<LevelEditor>();

        if (levelEditor != null)
        {
            // Se há Level Editor, reinicia o nível editado completamente
            gameOverPanel.SetActive(false);

            // Limpar todos os objetos (moedas coletadas, blocos destruídos, etc)
            // Destruir bombas ativas
            Bomba[] bombs = FindObjectsByType<Bomba>(FindObjectsSortMode.None);
            foreach (var bomb in bombs)
            {
                Destroy(bomb.gameObject);
            }

            // Destruir explosões ativas
            Explosao[] explosions = FindObjectsByType<Explosao>(FindObjectsSortMode.None);
            foreach (var explosion in explosions)
            {
                Destroy(explosion.gameObject);
            }
            Debug.Log($"[GameOverUI] Destroyed {bombs.Length} bombs and {explosions.Length} explosions");

            // Recarregar a fase salva quando clicou em Play
            levelEditor.LoadLevel("_current_level");
            Debug.Log("[GameOverUI] Reloaded saved level from Play button");

            // Resetar score
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.ResetScore();
            }

            // Atualizar NavGrid após reload
            NavGrid navGrid = FindFirstObjectByType<NavGrid>();
            if (navGrid != null)
            {
                navGrid.RefreshGrid();
            }

            // Resetar vida do jogador
            Player player = FindFirstObjectByType<Player>(FindObjectsInactive.Include);
            if (player != null)
            {
                // Reabilitar o GameObject do player
                player.gameObject.SetActive(true);

                // Usar reflection para acessar o campo privado 'health'
                var healthField = typeof(Player).GetField("health",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (healthField != null)
                {
                    int maxHealth = (int)healthField.GetValue(player);
                    player.currentHealth = maxHealth;
                }

                // Resetar posição do jogador para posição inicial
                player.transform.position = new Vector3(-6.5f, -0.5f, 0f);

                // Resetar movimento do Rigidbody2D
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }

                // Restaurar o modo de controle escolhido anteriormente
                levelEditor.RestorePlayerControlMode(player);

                Debug.Log("[GameOverUI] Player reactivated, health, position and movement reset");
            }

            // Resetar AI apenas se modo AI estava ativo
            if (!levelEditor.IsPlayerControlMode())
            {
                AIController[] aiControllers = FindObjectsByType<AIController>(FindObjectsSortMode.None);
                foreach (var ai in aiControllers)
                {
                    ai.enabled = false;
                    ai.enabled = true;
                }
                Debug.Log("[GameOverUI] AI reset for AI mode");
            }

            // Resetar timer se existir
            GameTimer timer = FindFirstObjectByType<GameTimer>();
            if (timer != null)
            {
                timer.ResetTimer();
                Debug.Log("[GameOverUI] Timer reset");
            }

            Time.timeScale = 1f;
            Debug.Log("[GameOverUI] Level completely restarted from saved state");
        }
        else
        {
            // Sem Level Editor, comportamento padrão (recarrega cena)
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // substitua pelo nome da cena de menu
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Fecha o painel de Game Over e ativa o Level Editor para editar a fase atual.
    /// </summary>
    public void EditLevel()
    {
        // Fecha o painel de Game Over
        gameOverPanel.SetActive(false);

        // Encontra o Level Editor na cena
        LevelEditor levelEditor = FindFirstObjectByType<LevelEditor>();

        if (levelEditor != null)
        {
            // Limpar bombas e explosões ativas antes de editar
            Bomba[] bombs = FindObjectsByType<Bomba>(FindObjectsSortMode.None);
            foreach (var bomb in bombs)
            {
                Destroy(bomb.gameObject);
            }

            Explosao[] explosions = FindObjectsByType<Explosao>(FindObjectsSortMode.None);
            foreach (var explosion in explosions)
            {
                Destroy(explosion.gameObject);
            }

            // Recarregar a fase salva
            levelEditor.LoadLevel("_current_level");
            Debug.Log("[GameOverUI] Reloaded saved level before editing");

            // Atualizar NavGrid
            NavGrid navGrid = FindFirstObjectByType<NavGrid>();
            if (navGrid != null)
            {
                navGrid.RefreshGrid();
            }

            // Resetar player: vida, posição e movimento
            Player player = FindFirstObjectByType<Player>(FindObjectsInactive.Include);
            if (player != null)
            {
                // Reabilitar o GameObject do player
                player.gameObject.SetActive(true);

                // Resetar vida usando reflection
                var healthField = typeof(Player).GetField("health",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (healthField != null)
                {
                    int maxHealth = (int)healthField.GetValue(player);
                    player.currentHealth = maxHealth;
                }

                // Resetar posição
                player.transform.position = new Vector3(-6.5f, -0.5f, 0f);

                // Resetar movimento do Rigidbody2D
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }

                Debug.Log("[GameOverUI] Player health, position and movement reset");
            }

            // Ativa o modo de edição
            levelEditor.EnableEditMode();
            Debug.Log("[GameOverUI] Level Editor enabled from Game Over screen");
        }
        else
        {
            Debug.LogWarning("[GameOverUI] Level Editor not found in scene!");
        }
    }
}
