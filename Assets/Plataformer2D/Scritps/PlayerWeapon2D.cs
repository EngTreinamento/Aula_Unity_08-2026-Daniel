using UnityEngine;

public class PlayerWeapon2D : MonoBehaviour
{
    //Referência do prefab do tiro
    [SerializeField] private GameObject bulletPrefab;
    //Ponto de onde o tiro vai sair
    [SerializeField] private Transform muzzle;

    [SerializeField] private float speed = 5;

    //Referência para a câmera
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Verificação de Input do mouse para atirar
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        //Pegamos a direção que o mouse está de acodo com player
        Vector2 direction = GetDirection();

        //Instanciamos o tiro no ponto que determinamos
        GameObject bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);
        //Pegamos uma referência do RigidBody2d do tiro
        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();

        //Determinamos o ângulo 
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        bullet.transform.eulerAngles = new Vector3(0, 0, angle);

        //Adicionamos a força no tiro
        bulletRB.AddForce(direction * speed, ForceMode2D.Impulse);
    }

    private Vector2 GetDirection()
    {
        //Pegamos a posição do mouse
        Vector3 screenPoint = Input.mousePosition;
        Debug.Log("Posição que o Mouse Retorna: " + screenPoint);

        // Vemos qual é a posição do mouse no mundo
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(screenPoint);
        Debug.Log("Posição que a câmera diz que o mouse está: " + mouseWorld);

        //Definimos a direção e enviamos ela normalizada
        Vector2 direction = 
            new Vector2(mouseWorld.x - transform.position.x, mouseWorld.y - transform.position.y);

        return direction.normalized;
    }
}
