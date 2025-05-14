using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Animator playerAnimator;
    private const float moveSpeed = 10f;
    [SerializeField] private CameraController cameraController;

    private UnityEngine.Vector2 movementInput;
    private CharacterController characterController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<UnityEngine.Vector2>();
        
        // Debug.Log(movementInput.x.ToString() + "   " + movementInput.y.ToString());
    }
    void FixedUpdate()
    {
        Vector3 direction = new Vector3(movementInput.x, 0f, movementInput.y);
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion camYaw = cameraController.GetCameraYawRotation();
            direction = camYaw * direction; // 카메라 방향으로 이동 벡터 회전

            characterController.Move(direction.normalized * moveSpeed * Time.deltaTime);

            // 캐릭터를 이동 방향으로 회전
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.15f);
        }

        playerAnimator.SetFloat("Horizontal", movementInput.x);
        playerAnimator.SetFloat("Vertical", movementInput.y);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}


