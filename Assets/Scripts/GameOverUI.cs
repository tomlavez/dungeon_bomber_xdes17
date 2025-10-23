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
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
}
