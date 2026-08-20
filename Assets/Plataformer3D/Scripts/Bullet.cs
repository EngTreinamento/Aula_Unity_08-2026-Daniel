using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Velocidade do tiro
    [SerializeField] float speed = 20f;
    //Tempo que ele vai durar caso não encoste em nada
    [SerializeField] float lifeTime = 5f;

    //Referência do dano que esse tiro vai dar
    private int damage;
    //Rigidbody para fazer o trajedo do dispado
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

    public void Setup(int damage)
    {
        this.damage = damage;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = (transform.forward * speed);
    }

    //Por enquanto, como não temos inimigo, quando ele colide com algo, ele simplesmente destroi
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
