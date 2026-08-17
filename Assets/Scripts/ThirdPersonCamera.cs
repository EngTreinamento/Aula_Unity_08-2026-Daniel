using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Vector3 offset;
    [SerializeField] Transform player;

    [SerializeField] float distance = 4f;
    [SerializeField] float sensitivity = 200f;
    [SerializeField] float pitchMin = -30f;
    [SerializeField] float pitchMax = 60f;
    [SerializeField] bool lockCursor = true;
    [SerializeField] float smoothSpeed = 12f;

    private float yaw;
    private float pitch;
    private Vector3 pivotSmoothed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        yaw = player.eulerAngles.y;
        pivotSmoothed = CalculatePivot();

        if (lockCursor == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        pivotSmoothed = Vector3.Lerp(pivotSmoothed, CalculatePivot(), smoothSpeed * Time.deltaTime);

        transform.position = pivotSmoothed + rotation * new Vector3(offset.x, 0f, -distance);

        transform.rotation = rotation;

        //transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, player.position.z + offset.z);
        //transform.position = Vector3.Lerp(transform.position, player.position + offset, Time.time * lerpSpeed)
        //transform.LookAt(player);
    }

    private Vector3 CalculatePivot()
    {
        return player.position + Vector3.up * offset.y;
    }
}
