using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    //Vetor para ser o nosso deslocamento de acordo com a posição do player que vai ficar a nossa câmera
    [SerializeField] Vector3 offset;
    //Referência do Transform do player
    [SerializeField] Transform player;

    //Controle de distância que a câmera considera para ser o lugar que a câmera vai olhar
    [SerializeField] float distance = 4f;
    //Controle de sensibilidade do mouse para rotacionar a câmera
    [SerializeField] float sensitivity = 200f;
    //Controle rotação vertical, pra impedir que a câmera dê cambalhotas
    [SerializeField] float pitchMin = -30f;
    [SerializeField] float pitchMax = 60f;
    //Controle se vamos ver o cursor ou não
    [SerializeField] bool lockCursor = true;
    //Controle de suavização para a câmera se deslocar até a posição desejada (Lerp)
    [SerializeField] float smoothSpeed = 12f;

    //Referência da rotação da câmera
    private float yaw;
    //Referência dp pivô Y da câmera
    private float pitch;
    //Referência de Vetor para fazer movimentação mais suave até o pivô
    private Vector3 pivotSmoothed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Pegamos a rotação em Y do nosso player
        yaw = player.eulerAngles.y;
        //Pegamos o cálculo do pivo inicial
        pivotSmoothed = CalculatePivot();

        //Se marcamos para não mostrar o cursos, ele já esconde
        if (lockCursor == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        //No yaw, nós acrecentamos o axis da movimentação no X junto a sensibilidade que determinamos e
        //o Time para 'controlar' essa variação de movientação
        yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        //Que fazemos o mesmo com o pitch, mas depois a faz o Clamp
        pitch -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        //Depois desses cálculos, nós calculamos qual será a nova rotação da câmera
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        //Feito isso, suavizamos a movimentação usando o Lerp
        pivotSmoothed = Vector3.Lerp(pivotSmoothed, CalculatePivot(), smoothSpeed * Time.deltaTime);

        //Por fim, aplicamos tanto a posição quanto a rotação na câmera
        transform.position = pivotSmoothed + rotation * new Vector3(offset.x, 0f, -distance);
        transform.rotation = rotation;
    }

    //Basicamente é pegar o pivô não necessariamente no meio do player, e sim pra onde vamos olhar acima ou ao lado dele
    private Vector3 CalculatePivot()
    {
        return player.position + Vector3.up * offset.y;
    }
}
