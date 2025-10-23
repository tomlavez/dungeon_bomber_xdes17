using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel; // seu painel criado

    void Start()
    {

    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainGame");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
