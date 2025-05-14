using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Distance & Zoom")]
    [SerializeField] private float distance = 6f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 10f;

    [Header("Rotation")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float pitchMin = -30f;
    [SerializeField] private float pitchMax = 60f;

    [Header("Smoothing")]
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float followSmoothTime = 0.05f;

    private float yaw = 0f;
    private float pitch = 15f;

    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 currentPosition;

    private Vector2 rotationSmoothVelocity;
    private Vector2 currentRotation;

    private void Start()
    {
        if (target == null) Debug.LogError("CameraController: Target이 설정되지 않았습니다.");
        currentPosition = transform.position;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LateUpdate()
    {
        HandleZoom();
        HandleRotation();

        Vector2 targetRotation = new Vector2(pitch, yaw);
        currentRotation = Vector2.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);

        Quaternion rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        Vector3 desiredPosition = target.position - (rotation * Vector3.forward * distance);

        currentPosition = Vector3.SmoothDamp(currentPosition, desiredPosition, ref currentVelocity, followSmoothTime);

        transform.position = currentPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        }
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    public Quaternion GetCameraYawRotation()
    {
        return Quaternion.Euler(0f, currentRotation.y, 0f);
    }
}
