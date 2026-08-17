using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 5;
    [SerializeField] float jumpForce = 5;
    [SerializeField] Transform cameraTransform;
    [SerializeField] bool useDoubleJump;

    Rigidbody rb;
    float moveX;
    float moveZ;
    Vector3 moveDirection;
    bool canJump = true;

    int qtdPulos = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        moveX = Input.GetAxis("Horizontal"); //AD
        moveZ = Input.GetAxis("Vertical"); //WS

        moveDirection = CalculateRelativeCameraDirection(moveX, moveZ);

        if (Input.GetButtonDown("Jump") && qtdPulos > 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            qtdPulos -= 1;
        }

        //transform.Translate(new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed);
        //rb.linearVelocity = new Vector3(moveX * speed, rb.linearVelocity.y, moveZ * speed);
    }

    private void FixedUpdate()
    {
        Vector3 velocity = moveDirection * speed;
        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;
    }

    private Vector3 CalculateRelativeCameraDirection(float x, float z)
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 rigth = cameraTransform.right;

        forward.y = 0;
        rigth.y = 0;

        forward.Normalize();
        rigth.Normalize();

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