using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] int chestValue = 100; // valor do baú
    [SerializeField] AudioClip somDaMoeda;

    public int GetChestValue()
    {
        return chestValue;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            AudioSource.PlayClipAtPoint(somDaMoeda, transform.position);

            // Adiciona pontos ao ScoreManager
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(chestValue);
            }

            // Destroi o baú
            Destroy(gameObject);
        }
    }
}
