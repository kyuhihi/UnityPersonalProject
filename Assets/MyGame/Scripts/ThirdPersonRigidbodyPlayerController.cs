using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class ThirdPersonRigidbodyPlayerController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CapsuleCollider")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;


        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private Rigidbody _rb;
        private StarterAssetsInputs _input;

        private AttackSystem _attackSystem;
        private GameObject _mainCamera;

        // Animation Hashes
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDMotionSpeed;
        private int _animIDVeritical;
        private int _animIDHorizontal;
        private int _animIDAttackCnt;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _speed;
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _jumpTimeoutDelta;
        private bool _hasAnimator;
        private const float _threshold = 0.01f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
            _input = GetComponent<StarterAssetsInputs>();
            _attackSystem = GetComponent<AttackSystem>();
            if (_mainCamera == null)
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget != null
                ? CinemachineCameraTarget.transform.rotation.eulerAngles.y
                : transform.rotation.eulerAngles.y;

            _hasAnimator = _animator != null;
            AssignAnimationIDs();
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            _jumpTimeoutDelta = JumpTimeout;
        }

        private void Update()
        {
            GroundedCheck();
            Move();
            Jump();
            UpdateAnimator();
        }
        private void FixedUpdate()
        {

        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDVeritical = Animator.StringToHash("Vertical");
            _animIDHorizontal = Animator.StringToHash("Horizontal");
            _animIDAttackCnt = Animator.StringToHash("AttackCnt");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            bool wasGrounded = Grounded;
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);

                // 공중에 떠있으면 FreeFall true, 땅에 있으면 false
                _animator.SetBool("FreeFall", !Grounded);

                // 착지 시 Jump false
                if (Grounded && !wasGrounded)
                {
                    _animator.SetBool(_animIDJump, false);
                }
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse() ? 1.0f : Time.deltaTime;
                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            if (CinemachineCameraTarget != null)
            {
                CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                    _cinemachineTargetYaw, 0.0f);
            }
        }
        private void Move()
        {
            if (_animator.GetInteger(_animIDAttackCnt) >= 0)
                return;

            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            _speed = Mathf.Lerp(_speed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);


            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // 회전은 좌/우 입력 있을 때만
            if (Mathf.Abs(_input.move.x) > 0.1f)
            {
                float turnAmount = _input.move.x;
                _targetRotation += turnAmount * 120f * Time.deltaTime;
                transform.rotation = Quaternion.Euler(0f, _targetRotation, 0f);
            }

            // 이동은 앞/뒤 입력 있을 때만 (z축 방향)
            if (Mathf.Abs(_input.move.y) > 0.1f)
            {
                Vector3 moveDirection = transform.forward * _input.move.y;
                Vector3 move = moveDirection.normalized * _speed;

                // Rigidbody 사용한 이동
                _rb.linearVelocity = new Vector3(move.x, _rb.linearVelocity.y, move.z);
            }
            else
            {
                // 정지 시 xz 평면 속도 0으로
                _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
            }
        }

        private void Jump()
        {
            if (Grounded && _input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z); // 점프 전에 Y속도 초기화
                _rb.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.Impulse);

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true); // 점프 시작
                    _animator.SetBool("FreeFall", false);
                }

                _jumpTimeoutDelta = JumpTimeout;
                _input.jump = false;
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }

            // 공중에 떠있으면 Jump false, FreeFall true
            if (_hasAnimator && !Grounded)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool("FreeFall", true);
            }
        }

        private void UpdateAnimator()
        {
            if (!_hasAnimator) return;
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, _input.move.magnitude);
            _animator.SetBool(_animIDGrounded, Grounded);


            _animator.SetFloat(_animIDVeritical, Mathf.Lerp(_animator.GetFloat(_animIDVeritical), _input.move.y * _speed, 0.15f));
            _animator.SetFloat(_animIDHorizontal, Mathf.Lerp(_animator.GetFloat(_animIDHorizontal), _input.move.x * _speed, 0.15f));

            // 점프 종료 타이밍을 더 정확히
            if (!_input.jump && Grounded)
                _animator.SetBool(_animIDJump, false);
        }
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private bool IsCurrentDeviceMouse()
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput != null && _playerInput.currentControlScheme == "KeyboardMouse";
#else
            return false;
#endif
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = Grounded ? transparentGreen : transparentRed;
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.position, FootstepAudioVolume);
            }
        }
        
    }
}