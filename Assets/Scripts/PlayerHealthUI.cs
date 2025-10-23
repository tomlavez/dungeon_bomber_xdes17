using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Player player; // Arraste o Player no Inspector
    public Transform heartsParent; // Arraste o HealthUI (pai dos corações)

    private Image[] hearts; // Array de imagens de coração

    void Awake()
    {
        // Pega todos os Images filhos do HealthUI
        hearts = heartsParent.GetComponentsInChildren<Image>();
    }

    void Update()
    {
        // Atualiza os corações de acordo com a vida atual do player
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < player.currentHealth) // compara com a vida atual
                hearts[i].enabled = true; // mostra coração
            else
                hearts[i].enabled = false; // esconde coração
        }
    }
}
