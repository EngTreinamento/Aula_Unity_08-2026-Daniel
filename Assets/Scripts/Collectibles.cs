using UnityEngine;

public class Collectibles : MonoBehaviour
{
    //Referência da Data de cada coletável, onde podemos mudar qual vai ser o score e a aparência de cada coletável
    [SerializeField] CollectibleData collectible;

    void Start()
    {
        GetComponent<MeshRenderer>().material.color = collectible.color;
        Debug.Log("Total de Pontos que esse coletável vai dar: " +  collectible.score);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Métodos de colisão para verificar se o player encostou no coletável,
    //para daí fazer as suas ações no OnPlayerCollision, tanto trigger quando colisão padrão
    private void OnCollisionEnter(Collision collision)
    {
        //Estamos verificando a colisão por tag, pois o player não vai fazer nada quando isso colidir,
        //apenas o coletável
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Através de Colisão normal");
            OnPlayerCollision();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Estamos verificando a colisão por tag, pois o player não vai fazer nada quando isso colidir,
        //apenas o coletável
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Através de Colisão trigger");
            OnPlayerCollision();
        }
    }

    private void OnPlayerCollision()
    {
        // Primeiro ele vai procurar o ScoreManager na cena
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

        // Depois ele vai usar o método do ScoreManager para adicionar a pontuação
        scoreManager.AddScore(collectible.score);

        // Por fim, ele vai destruir o coletável para limpar a cena
        Destroy(gameObject);
    }
}
