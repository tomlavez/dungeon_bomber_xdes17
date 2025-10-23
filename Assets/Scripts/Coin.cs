using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] int coinValue = 10;
    [SerializeField] AudioClip somDaMoeda;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            AudioSource.PlayClipAtPoint(somDaMoeda, transform.position);

            // Adiciona pontos ao ScoreManager
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(coinValue);
            }

            // Destroi a moeda
            Destroy(gameObject);
        }
    }
}
