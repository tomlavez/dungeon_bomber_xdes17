using UnityEngine;

public class Bomba : MonoBehaviour
{

    public float tempoParaExplodir = 3f;
    public GameObject prefabDaExplosao;

    [SerializeField] AudioClip somDaExplosao;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("Explodir", tempoParaExplodir);
    }

    void Explodir()
    {
        // Toca o som da explosão
        AudioSource.PlayClipAtPoint(somDaExplosao, transform.position);

        Instantiate(prefabDaExplosao, transform.position, Quaternion.identity);

        // Explosao em cruz
        Instantiate(prefabDaExplosao, transform.position + Vector3.up, Quaternion.identity);
        Instantiate(prefabDaExplosao, transform.position + Vector3.down, Quaternion.identity);
        Instantiate(prefabDaExplosao, transform.position + Vector3.left, Quaternion.identity);
        Instantiate(prefabDaExplosao, transform.position + Vector3.right, Quaternion.identity);

        Destroy(gameObject);
    }
}
