using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // singleton para fácil acesso
    public int score = 0; // pontuação atual
    public TextMeshProUGUI scoreText; // UI Text para mostrar o score

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreUI();
    }

    // Função para adicionar pontos
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    // Atualiza a UI
    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}
