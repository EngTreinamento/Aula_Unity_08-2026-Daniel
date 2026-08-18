using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Variável para controlar a velocidade
    [SerializeField] float speed = 5;
    //Variável para controlar o impulso do pulo, pra ir mais alto ou mais baixo
    [SerializeField] float jumpForce = 5;
    // Referência da câmera para movimentar o jogador de acordo com a direção que está olhando
    [SerializeField] Transform cameraTransform;
    // Booelana para habilitar ou não o duplo
    [SerializeField] bool useDoubleJump;

    // Usando o RigidBody para movimentação que por padrão já é para movimentação 3D
    Rigidbody rb;
    //Referência do PlayerWeapon para saber quem está controlando a visualização
    PlayerWeapon playerWeapon;
    // Duas referências de eixo para movimentar o personagem
    float moveX;
    float moveZ;

    //Vetor para calcular a direção que vai ser a minha 'nova frente'
    Vector3 moveDirection;
    // Contador de pulos para conversar com o pulo duplo
    int qtdPulos = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Fazemos apenas os 'Getters' do que precisamos de referência da cena
        rb = GetComponent<Rigidbody>();
        playerWeapon = GetComponent<PlayerWeapon>();
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //Pegar os Axis para saber qual lado estamos andando
        moveX = Input.GetAxis("Horizontal"); // Teclas A e D ou setas Esquerda e Direita
        moveZ = Input.GetAxis("Vertical"); // Teclas W e S ou setas para cima e para baixo

        // Chama o método para saber qual a direção que será a nova frente do jogador
        moveDirection = CalculateRelativeCameraDirection(moveX, moveZ);

        //Para a lógica do pulo, verificamos em conjunto se o jogador apertou a tecla de pulo, e se ele pode pular
        if (Input.GetButtonDown("Jump") && qtdPulos > 0)
        {
            //Quando ele está apto a pular, ele aplica um impulso para cima, de acordo com a força do pulo
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //E reduz a quantidade de pulos que ele pode fazer.
            qtdPulos -= 1;
        }
    }

    //Para movimentação, o ideal é usar o FixedUpdate, pois ele vai atualizar com uma taxa de quadros fixas.
    //Diferente do Update, onde ele atualiza a cada taxa de quadros do desempenho do computador
    private void FixedUpdate()
    {
        // Criamos um Vector3 pra ser a velocidade, onde multiplicamos a direção pela velocidade
        Vector3 velocity = moveDirection * speed;

        // No caso do eixo Y, a gente mantém o mesmo valor que já está na velocidade dele,
        // pois se não fizermos isso, o jogador pode "andar no ar"
        velocity.y = rb.linearVelocity.y;

        //Por fim, aplicamos a velocidade no RigidBody
        rb.linearVelocity = velocity;
    }

    // Nesse método, nós fazemos o cálculo para saber qual é a nova frente do jogador,
    // considerando para onde a câmera está olhando
    // Para isso, vamos precisar dos axis do jogador, ou seja, quais teclas o jogador apertou
    private Vector3 CalculateRelativeCameraDirection(float x, float z)
    {
        //Nós pegamos os vetores da frente da câmera e da diretita para sabermos a rotação dela
        Vector3 forward = cameraTransform.forward;
        Vector3 rigth = cameraTransform.right;

        // zeramos o Y para não ter nenhum ruído na hora de converter a frente e deixar a mira no meio
        forward.y = 0;
        rigth.y = 0;

        // Vector3(1000, 500, 250) - (1, 0.5, 0.25)

        //Nós normalizamos esses vetores
        forward.Normalize();
        rigth.Normalize();

        // Por fim, a gente faz um cálculo para passar a direção correta que temos que andar.
        return Vector3.ClampMagnitude(forward * z + rigth * x, 1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (useDoubleJump)
            {
                qtdPulos = 2;
            }
            else
            {
                qtdPulos = 1;
            }
        }
    }
}