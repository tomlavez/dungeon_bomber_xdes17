using UnityEngine;

public class GameModeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelEditor levelEditor;
    [SerializeField] private GameObject player;

    [Header("Controllers (Opcional)")]
    [SerializeField] private MonoBehaviour aiController;
    [SerializeField] private MonoBehaviour playerKeyboardController;

    [Header("Optional Components")]
    [SerializeField] private MonoBehaviour scoreManager;
    [SerializeField] private MonoBehaviour gameTimer;

    /// <summary>
    /// Inicia o modo de edição.
    /// Pausa o jogo e desativa controladores.
    /// </summary>
    public void StartEditMode()
    {
        if (levelEditor != null)
        {
            levelEditor.EnableEditMode();
        }

        // Desativar IA e ativar controle manual (se preferir)
        if (aiController != null)
            aiController.enabled = false;

        if (playerKeyboardController != null)
            playerKeyboardController.enabled = true;

        Debug.Log("[GameMode] Edit Mode Started");
    }

    /// <summary>
    /// Inicia o modo de jogo com IA.
    /// Resume o jogo e ativa a IA.
    /// </summary>
    public void StartPlayModeWithAI()
    {
        if (levelEditor != null)
        {
            levelEditor.DisableEditMode();
        }

        // Ativar IA e desativar controle manual
        if (aiController != null)
            aiController.enabled = true;

        if (playerKeyboardController != null)
            playerKeyboardController.enabled = false;

        // Resetar componentes opcionais
        ResetGameComponents();

        Debug.Log("[GameMode] Play Mode Started (AI Active)");
    }

    /// <summary>
    /// Inicia o modo de jogo com controle manual.
    /// Resume o jogo e ativa controle de teclado.
    /// </summary>
    public void StartPlayModeWithKeyboard()
    {
        if (levelEditor != null)
        {
            levelEditor.DisableEditMode();
        }

        // Desativar IA e ativar controle manual
        if (aiController != null)
            aiController.enabled = false;

        if (playerKeyboardController != null)
            playerKeyboardController.enabled = true;

        // Resetar componentes opcionais
        ResetGameComponents();

        Debug.Log("[GameMode] Play Mode Started (Keyboard Active)");
    }

    /// <summary>
    /// Reseta componentes do jogo (score, timer, etc).
    /// </summary>
    private void ResetGameComponents()
    {
        // Resetar ScoreManager se existir
        if (scoreManager != null)
        {
            var resetMethod = scoreManager.GetType().GetMethod("ResetScore");
            if (resetMethod != null)
            {
                resetMethod.Invoke(scoreManager, null);
            }
        }

        // Resetar GameTimer se existir
        if (gameTimer != null)
        {
            var resetMethod = gameTimer.GetType().GetMethod("ResetTimer");
            if (resetMethod != null)
            {
                resetMethod.Invoke(gameTimer, null);
            }
        }

        // Resetar posição do player se necessário
        if (player != null)
        {
            // Voltar para posição inicial (ajuste conforme necessário)
            // player.transform.position = initialPosition;
        }
    }

    /// <summary>
    /// Alterna entre modo edit e play com IA.
    /// </summary>
    public void ToggleMode()
    {
        if (levelEditor != null && Time.timeScale == 0f)
        {
            // Está em edit mode, ir para play mode
            StartPlayModeWithAI();
        }
        else
        {
            // Está em play mode, ir para edit mode
            StartEditMode();
        }
    }

    void Update()
    {
        // Atalhos de teclado opcionais
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartEditMode();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            StartPlayModeWithAI();
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            StartPlayModeWithKeyboard();
        }
    }
}
