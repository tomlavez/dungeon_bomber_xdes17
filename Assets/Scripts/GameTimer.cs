using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] float startTime = 60f;
    [SerializeField] TextMeshProUGUI timerText;

    float currentTime;
    bool timerRunning = true;
    bool gameEnded = false;

    void Start()
    {
        currentTime = startTime;
        UpdateTimerUI();
    }

    void Update()
    {
        if (!timerRunning || gameEnded) return;

        currentTime -= Time.deltaTime;

        // Evita valores negativos
        if (currentTime < 0f)
            currentTime = 0f;

        UpdateTimerUI();

        if (currentTime <= 0f)
        {
            timerRunning = false;
            EndGame();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";

            timerText.color = currentTime <= 10f ? Color.red : Color.white;
        }
    }

    void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        GameOverUI gameOverUI = FindFirstObjectByType<GameOverUI>(FindObjectsInactive.Include);
        if (gameOverUI != null)
        {
            int finalScore = ScoreManager.instance != null ? ScoreManager.instance.score : 0;
            gameOverUI.ShowGameOver(finalScore);
            Debug.Log("⏰ Tempo esgotado! Mostrando Game Over.");
        }

        Time.timeScale = 0f;
    }
}
